namespace Infrastructure.AzureContainerApps.Sample.Helpers;

internal static class Naming
{
    public static string BuildAcrName(string stackSuffix)
    {
        var acrName = $"acasmp{stackSuffix}";
        if (acrName.Length > 50)
        {
            acrName = acrName[..50];
        }

        return acrName;
    }
}

