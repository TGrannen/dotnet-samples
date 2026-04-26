using System.Text;

namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class Naming
{
    public static string ToAcrSafeSuffix(string input)
    {
        var sb = new StringBuilder();
        foreach (var ch in input.ToLowerInvariant())
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
            {
                sb.Append(ch);
            }
        }

        return sb.Length == 0 ? "dev" : sb.ToString();
    }

    public static string BuildAcrName(string stackSuffix)
    {
        var acrName = $"acasmp{stackSuffix}";
        if (acrName.Length > 50)
        {
            acrName = acrName[..50];
        }

        return acrName;
    }

    public static string BuildAppName(string stackSuffix) => $"aca-sample-api-{stackSuffix}";
}

