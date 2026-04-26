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
    var cfg = SampleConfig.FromPulumiConfig(new Config());

    var stackSuffix = Naming.ToAcrSafeSuffix(Pulumi.Deployment.Instance.StackName);
    var acrName = Naming.BuildAcrName(stackSuffix);

    var resourceGroup = new ResourceGroup("aca-sample-rg", new ResourceGroupArgs
    {
        Location = cfg.Location,
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

    var (_, _, appLogsConfiguration) = LogAnalyticsSupport.MaybeCreate(cfg.EnableLogAnalytics, resourceGroup);

    var environmentArgs = new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
    };

    if (appLogsConfiguration is not null)
    {
        environmentArgs.AppLogsConfiguration = appLogsConfiguration;
    }

    var environment = new ManagedEnvironment("aca-sample-env", environmentArgs);

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

    var appName = Naming.BuildAppName(stackSuffix);

    // Always keep the Container App in the Pulumi state so infra-only runs don't delete it.
    // When deployApp=false, we "freeze" it by protecting it from deletion and ignoring changes,
    // so the stack can still update infra (RG/ACR/env/identity) safely.

    var containerApp = ContainerAppSupport.CreateContainerApp(
        appName: appName,
        cfg: cfg,
        resourceGroupName: resourceGroup.Name,
        location: resourceGroup.Location,
        environment: environment,
        acr: acr,
        pullIdentity: pullIdentity);

    var latestRevisionName = containerApp.LatestRevisionName;
    var stableRevisionName = string.IsNullOrWhiteSpace(cfg.StableRevisionName)
        ? latestRevisionName
        : Output.Create(cfg.StableRevisionName);

    var stableUrl = ContainerAppSupport.StableUrl(appName, environment);
    var latestRevisionUrl = ContainerAppSupport.LatestRevisionUrl(containerApp);

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["acrName"] = acr.Name,
        ["acrLoginServer"] = acr.LoginServer,
        ["containerAppName"] = appName,
        ["latestRevisionName"] = latestRevisionName,
        ["stableRevisionName"] = stableRevisionName,
        ["stableUrl"] = stableUrl,
        ["latestRevisionUrl"] = latestRevisionUrl,
    };
});