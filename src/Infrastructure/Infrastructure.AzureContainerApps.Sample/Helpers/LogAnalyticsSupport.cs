using Pulumi;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.App.Inputs;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class LogAnalyticsSupport
{
    public static (Workspace? Workspace, Output<string>? SharedKey, AppLogsConfigurationArgs? AppLogsConfiguration) MaybeCreate(
        bool enableLogAnalytics,
        ResourceGroup resourceGroup)
    {
        if (!enableLogAnalytics)
        {
            return (Workspace: null, SharedKey: null, AppLogsConfiguration: null);
        }

        var workspace = new Workspace("acasamplelaw", new WorkspaceArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Sku = new WorkspaceSkuArgs
            {
                Name = "PerGB2018",
            },
            // Retention has a cost impact; 30 is the common minimum depending on SKU/region.
            RetentionInDays = 30,
        });

        var sharedKeys = GetSharedKeys.Invoke(new GetSharedKeysInvokeArgs
        {
            ResourceGroupName = resourceGroup.Name,
            WorkspaceName = workspace.Name,
        });

        var sharedKey = sharedKeys.Apply(k => k.PrimarySharedKey!);

        var appLogsConfiguration = new AppLogsConfigurationArgs
        {
            Destination = "log-analytics",
            LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
            {
                CustomerId = workspace.CustomerId,
                SharedKey = sharedKey,
            },
        };

        return (Workspace: workspace, SharedKey: sharedKey, AppLogsConfiguration: appLogsConfiguration);
    }
}

