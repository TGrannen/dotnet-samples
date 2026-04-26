using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class ContainerAppSupport
{
    // Exists on MCR without ACR; used when DeployApp is false so the first up succeeds before CI pushes sample-api.
    private const string PlaceholderImage = "mcr.microsoft.com/azuredocs/containerapps-helloworld";

    public static ContainerApp CreateContainerApp(
        string appName,
        SampleConfig cfg,
        Output<string> resourceGroupName,
        Output<string> location,
        ManagedEnvironment environment,
        Registry acr,
        UserAssignedIdentity pullIdentity)
    {
        var traffic = BuildTraffic(cfg.StableRevisionName);

        // Infra-only runs must not reference ACR tags that do not exist yet (IgnoreChanges does not apply on create).
        var image = cfg.DeployApp
            ? Output.Format($"{acr.LoginServer}/{cfg.ImageName}:{cfg.ImageTag}")
            : Output.Create(PlaceholderImage);

        return new ContainerApp(appName, new ContainerAppArgs
        {
            ResourceGroupName = resourceGroupName,
            Location = location,
            ManagedEnvironmentId = environment.Id,
            Configuration = new ConfigurationArgs
            {
                ActiveRevisionsMode = ActiveRevisionsMode.Multiple,
                Ingress = new IngressArgs
                {
                    External = true,
                    TargetPort = cfg.ContainerPort,
                    Transport = IngressTransportMethod.Auto,
                    Traffic = traffic,
                },
                Registries =
                {
                    new RegistryCredentialsArgs
                    {
                        Server = acr.LoginServer,
                        Identity = pullIdentity.Id,
                    },
                },
            },
            Identity = new Pulumi.AzureNative.Commontypesv5.Inputs.ManagedServiceIdentityArgs
            {
                Type = "UserAssigned",
                UserAssignedIdentities =
                {
                    pullIdentity.Id,
                },
            },
            Template = new TemplateArgs
            {
                Containers =
                {
                    new ContainerArgs
                    {
                        Name = "api",
                        Image = image,
                        Env =
                        {
                            new EnvironmentVarArgs
                            {
                                Name = "ASPNETCORE_URLS",
                                Value = $"http://0.0.0.0:{cfg.ContainerPort}",
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "APP_VERSION_SHA",
                                Value = cfg.ImageTag,
                            },
                        },
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = cfg.Cpu,
                            Memory = cfg.Memory,
                        },
                    },
                },
                Scale = new ScaleArgs
                {
                    // Cheapest behavior for samples: allow scale-to-zero.
                    // With ingress enabled, ACA will use HTTP scaling by default.
                    MinReplicas = 0,
                    MaxReplicas = 1,
                },
            },
        }, new CustomResourceOptions
        {
            IgnoreChanges = cfg.DeployApp
                ? []
                :
                [
                    "configuration",
                    "template",
                    "identity"
                ],
        });
    }

    public static Output<string> StableUrl(string appName, ManagedEnvironment environment) =>
        Output.Format($"https://{appName}.{environment.DefaultDomain}");

    public static Output<string> LatestRevisionUrl(ContainerApp app) =>
        app.LatestRevisionFqdn.Apply(fqdn => $"https://{fqdn}");

    private static TrafficWeightArgs[] BuildTraffic(string? stableRevisionNameConfig)
    {
        if (string.IsNullOrWhiteSpace(stableRevisionNameConfig))
        {
            return
            [
                new TrafficWeightArgs
                {
                    Label = "stable",
                    LatestRevision = true,
                    Weight = 100,
                }
            ];
        }

        return
        [
            new TrafficWeightArgs
            {
                Label = "stable",
                RevisionName = stableRevisionNameConfig,
                Weight = 100,
            },
            new TrafficWeightArgs
            {
                Label = "staging",
                LatestRevision = true,
                Weight = 0,
            }
        ];
    }
}