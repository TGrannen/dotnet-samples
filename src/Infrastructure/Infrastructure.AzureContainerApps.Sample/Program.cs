using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Infrastructure.AzureContainerApps.Sample.Helpers;

return await Pulumi.Deployment.RunAsync(() =>
{
    var cfg = SampleConfig.FromPulumiConfig(new Config(), Pulumi.Deployment.Instance.StackName);

    var resourceGroup = new ResourceGroup("aca-sample-rg", new ResourceGroupArgs
    {
        Location = cfg.Location
    });

    var acr = new Registry(Naming.BuildAcrName(cfg.AcrSafeSuffix), new RegistryArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Sku = new SkuArgs
        {
            Name = SkuName.Basic
        },
        AdminUserEnabled = false
    });

    var (_, _, appLogsConfiguration) = LogAnalyticsSupport.MaybeCreate(cfg.EnableLogAnalytics, resourceGroup);

    var environmentArgs = new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location
    };

    if (appLogsConfiguration is not null)
    {
        environmentArgs.AppLogsConfiguration = appLogsConfiguration;
    }

    var environment = new ManagedEnvironment("aca-sample-env", environmentArgs);

    var pullIdentity = new UserAssignedIdentity("aca-sample-pull-identity", new UserAssignedIdentityArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location
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
        Scope = acr.Id
    });

    // Container app shell (ingress, registry identity, scale mode) is managed here; image and traffic weights are updated in CI via Azure CLI.
    var app = ContainerAppSupport.CreateContainerApp(cfg: cfg,
        resourceGroupName: resourceGroup.Name,
        location: resourceGroup.Location,
        environment: environment,
        acr: acr,
        pullIdentity: pullIdentity);

    var stableUrl = ContainerAppSupport.StableUrl(app.Name, environment);

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["acrName"] = acr.Name,
        ["acrLoginServer"] = acr.LoginServer,
        ["containerAppName"] = app.Name,
        ["stableUrl"] = stableUrl
    };
});