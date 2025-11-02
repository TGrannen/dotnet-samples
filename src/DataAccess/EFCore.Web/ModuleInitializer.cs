using System.Runtime.CompilerServices;

namespace EFCore.Web;

public class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AutoFaker.Configure(builder => { builder.WithConventions(); });
        Randomizer.Seed = new Random(8675309);
    }
}