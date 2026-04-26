using Pulumi;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal sealed record SampleConfig(
    string Location,
    bool DeployApp,
    bool EnableLogAnalytics,
    string ImageName,
    string ImageTag,
    int ContainerPort,
    double Cpu,
    string Memory,
    bool IsLive,
    string? StableRevisionName)
{
    public static SampleConfig FromPulumiConfig(Config config)
    {
        var location = config.Get("azure-native:location") ?? config.Get("location") ?? "eastus";
        var deployApp = config.GetBoolean("deployApp") ?? true;
        var enableLogAnalytics = config.GetBoolean("enableLogAnalytics") ?? false;

        var imageName = config.Get("imageName") ?? "sample-api";
        var imageTag = config.Get("imageTag") ?? "dev";
        var containerPort = config.GetInt32("containerPort") ?? 8080;

        var cpu = config.GetDouble("cpu") ?? 0.25;
        var memory = config.Get("memory") ?? "0.5Gi";

        var isLive = config.GetBoolean("isLive") ?? false;
        var stableRevisionName = config.Get("stableRevisionName"); // may be null on first run

        return new SampleConfig(
            Location: location,
            DeployApp: deployApp,
            EnableLogAnalytics: enableLogAnalytics,
            ImageName: imageName,
            ImageTag: imageTag,
            ContainerPort: containerPort,
            Cpu: cpu,
            Memory: memory,
            IsLive: isLive,
            StableRevisionName: stableRevisionName);
    }
}

