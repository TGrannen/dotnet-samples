using System.Diagnostics;

namespace Caching.WebAPI;

class Tracing
{
    public static string SourceName = "Caching.Example";
    public static readonly ActivitySource Source = new(SourceName, "1.0.0");
}