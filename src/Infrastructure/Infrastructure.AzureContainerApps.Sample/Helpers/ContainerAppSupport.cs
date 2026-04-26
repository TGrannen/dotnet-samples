using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class ContainerAppSupport
{
    public static TrafficWeightArgs[] BuildTraffic(string? stableRevisionNameConfig)
    {
        return string.IsNullOrWhiteSpace(stableRevisionNameConfig)
            ? new[]
            {
                new TrafficWeightArgs
                {
                    Label = "stable",
                    LatestRevision = true,
                    Weight = 100,
                },
            }
            : new[]
            {
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
                },
            };
    }

    public static CustomResourceOptions BuildContainerAppOptions(bool deployApp)
    {
        return new CustomResourceOptions
        {
            Protect = !deployApp,
            IgnoreChanges = deployApp
                ? new List<string>()
                : new List<string>
                {
                    "configuration",
                    "template",
                    "identity",
                },
        };
    }

    public static ContainerApp CreateContainerApp(
        string appName,
        SampleConfig cfg,
        Output<string> resourceGroupName,
        Output<string> location,
        ManagedEnvironment environment,
        Registry acr,
        UserAssignedIdentity pullIdentity,
        TrafficWeightArgs[] traffic,
        CustomResourceOptions options)
    {
        var image = Output.Format($"{acr.LoginServer}/{cfg.ImageName}:{cfg.ImageTag}");

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
        }, options);
    }

    public static Output<string> StableUrl(string appName, ManagedEnvironment environment) =>
        Output.Format($"https://{appName}.{environment.DefaultDomain}");

    public static Output<string> LatestRevisionUrl(ContainerApp app) =>
        app.LatestRevisionFqdn.Apply(fqdn => $"https://{fqdn}");
}

