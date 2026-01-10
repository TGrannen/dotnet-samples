namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public class PlaywrightOptions 
{
    public const string SectionName = "Playwright";
    public bool Debug { get; set; }
    public bool Headed { get; set; }
    public TimeSpan? SlowMo { get; set; } = null;
    public bool ReuseContext { get; set; } = true;
}