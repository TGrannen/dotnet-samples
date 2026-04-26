using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class ContainerAppSupport
{
    // MCR image so the first `pulumi up` succeeds before CI pushes to ACR. Image updates are done with Azure CLI.
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
                    Traffic =
                    [
                        new TrafficWeightArgs
                        {
                            LatestRevision = true,
                            Weight = 100,
                        },
                    ],
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
                        Image = PlaceholderImage,
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
                                Value = "pulumi-placeholder",
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
            // CI updates image, env, scale, and traffic weights via Azure CLI; do not reconcile those from Pulumi.
            IgnoreChanges =
            [
                "template",
                "configuration.ingress.traffic",
            ],
        });
    }

    public static Output<string> StableUrl(string appName, ManagedEnvironment environment) =>
        Output.Format($"https://{appName}.{environment.DefaultDomain}");
}