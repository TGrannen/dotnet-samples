using System.Text;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Config();

    var location = config.Get("azure-native:location") ?? config.Get("location") ?? "eastus";
    var deployApp = config.GetBoolean("deployApp") ?? true;

    var imageName = config.Get("imageName") ?? "sample-api";
    var imageTag = config.Get("imageTag") ?? "dev";
    var containerPort = config.GetInt32("containerPort") ?? 8080;

    var cpu = config.GetDouble("cpu") ?? 0.25;
    var memory = config.Get("memory") ?? "0.5Gi";

    var isLive = config.GetBoolean("isLive") ?? false;
    var stableRevisionNameConfig = config.Get("stableRevisionName"); // may be null on first run

    static string ToAcrSafeSuffix(string input)
    {
        var sb = new StringBuilder();
        foreach (var ch in input.ToLowerInvariant())
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
                sb.Append(ch);
        }
        return sb.Length == 0 ? "dev" : sb.ToString();
    }

    var stackSuffix = ToAcrSafeSuffix(Pulumi.Deployment.Instance.StackName);
    var acrName = $"acasmp{stackSuffix}";
    if (acrName.Length > 50) acrName = acrName[..50];

    var resourceGroup = new ResourceGroup("aca-sample-rg", new ResourceGroupArgs
    {
        Location = location,
    });

    var acr = new Registry(acrName, new RegistryArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Sku = new SkuArgs
        {
            Name = SkuName.Basic,
        },
        AdminUserEnabled = false,
    });

    var workspace = new Workspace("acasamplelaw", new WorkspaceArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Sku = new WorkspaceSkuArgs
        {
            Name = "PerGB2018",
        },
        RetentionInDays = 30,
    });

    var sharedKeys = GetSharedKeys.Invoke(new GetSharedKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        WorkspaceName = workspace.Name,
    });

    var environment = new ManagedEnvironment("aca-sample-env", new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        AppLogsConfiguration = new AppLogsConfigurationArgs
        {
            Destination = "log-analytics",
            LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
            {
                CustomerId = workspace.CustomerId,
                SharedKey = sharedKeys.Apply(k => k.PrimarySharedKey!),
            },
        },
    });

    var pullIdentity = new UserAssignedIdentity("aca-sample-pull-identity", new UserAssignedIdentityArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
    });

    // Allow the ACA managed identity to pull from ACR.
    var acrPullRoleDefinitionId = acr.Id.Apply(acrId =>
        // Built-in role definition ID for AcrPull:
        // 7f951dda-4ed3-4680-a7ca-43fe172d538d
        $"{acrId}/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d");

    _ = new RoleAssignment($"aca-sample-acrpull-{Pulumi.Deployment.Instance.StackName}", new RoleAssignmentArgs
    {
        PrincipalId = pullIdentity.PrincipalId,
        PrincipalType = PrincipalType.ServicePrincipal,
        RoleDefinitionId = acrPullRoleDefinitionId,
        Scope = acr.Id,
    });

    var appName = $"aca-sample-api-{stackSuffix}";

    var image = Output.Format($"{acr.LoginServer}/{imageName}:{imageTag}");

    Output<string>? latestRevisionName = null;
    Output<string>? stableRevisionName = null;
    Output<string>? stableUrl = null;
    Output<string>? latestRevisionUrl = null;

    if (deployApp)
    {
        // Create/update the Container App. Updating the image creates a new revision.
        var traffic = string.IsNullOrWhiteSpace(stableRevisionNameConfig)
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

        var containerApp = new ContainerApp(appName, new ContainerAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ManagedEnvironmentId = environment.Id,
            Configuration = new ConfigurationArgs
            {
                ActiveRevisionsMode = ActiveRevisionsMode.Multiple,
                Ingress = new IngressArgs
                {
                    External = true,
                    TargetPort = containerPort,
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
                                Value = $"http://0.0.0.0:{containerPort}",
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "APP_VERSION_SHA",
                                Value = imageTag,
                            },
                        },
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = cpu,
                            Memory = memory,
                        },
                    },
                },
                Scale = new ScaleArgs
                {
                    MinReplicas = 1,
                    MaxReplicas = 2,
                },
            },
        });

        latestRevisionName = containerApp.LatestRevisionName;
        stableRevisionName = string.IsNullOrWhiteSpace(stableRevisionNameConfig)
            ? latestRevisionName
            : Output.Create(stableRevisionNameConfig);

        stableUrl = Output.Format($"https://{appName}.{environment.DefaultDomain}");
        // Always reachable, regardless of traffic weights / labels.
        latestRevisionUrl = containerApp.LatestRevisionFqdn.Apply(fqdn => $"https://{fqdn}");
    }

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["acrName"] = acr.Name,
        ["acrLoginServer"] = acr.LoginServer,
        ["containerAppName"] = deployApp ? appName : null,
        ["latestRevisionName"] = latestRevisionName,
        ["stableRevisionName"] = stableRevisionName,
        ["stableUrl"] = stableUrl,
        ["latestRevisionUrl"] = latestRevisionUrl,
    };
});

