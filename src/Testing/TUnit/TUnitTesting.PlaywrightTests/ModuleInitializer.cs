using System.Runtime.CompilerServices;

namespace TUnitTesting.PlaywrightTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        if (Environment.GetEnvironmentVariable("PLAYWRIGHT_SKIP_INSTALL") == "1")
            return;

        Program.Main(["install"]);
    }
}