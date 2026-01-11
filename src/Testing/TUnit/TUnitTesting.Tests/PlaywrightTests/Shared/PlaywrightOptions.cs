namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public class PlaywrightOptions
{
    public const string SectionName = "Playwright";
    public bool Debug { get; set; }
    public bool Headed { get; set; }
    public TimeSpan? SlowMo { get; set; } = null;
    public bool ReuseContext { get; set; } = true;
    public TracingOptions Tracing { get; set; } = new();
}

public class TracingOptions
{
    public bool Enabled { get; set; }
    public bool Screenshots { get; set; } = true;
    public bool Snapshots { get; set; } = true;
    public bool Sources { get; set; } = true;
    public string? OutputDirectory { get; set; }
}