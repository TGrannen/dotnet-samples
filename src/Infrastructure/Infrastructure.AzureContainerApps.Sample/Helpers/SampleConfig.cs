using System.Text;
using Pulumi;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal sealed record SampleConfig(
    string Location,
    bool EnableLogAnalytics,
    int ContainerPort,
    double Cpu,
    string Memory,
    string AcrSafeSuffix)
{
    public static string ToAcrSafeSuffix(string input)
    {
        var sb = new StringBuilder();
        foreach (var ch in input.ToLowerInvariant())
        {
            if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                sb.Append(ch);
            }
        }

        return sb.Length == 0 ? "dev" : sb.ToString();
    }

    public static SampleConfig FromPulumiConfig(Config config, string instanceStackName)
    {
        var location = config.Get("azure-native:location") ?? config.Get("location") ?? "eastus";
        var enableLogAnalytics = config.GetBoolean("enableLogAnalytics") ?? false;

        var containerPort = config.GetInt32("containerPort") ?? 8080;

        var cpu = config.GetDouble("cpu") ?? 0.25;
        var memory = config.Get("memory") ?? "0.5Gi";

        return new SampleConfig(
            Location: location,
            EnableLogAnalytics: enableLogAnalytics,
            ContainerPort: containerPort,
            Cpu: cpu,
            Memory: memory,
            ToAcrSafeSuffix(instanceStackName));
    }
}