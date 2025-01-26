namespace PlaywrightTests.Setup;

public abstract class PageBase : ConfigurationBase
{
    private IPlaywright? _pw;
    private IBrowser? _browser;
    private IBrowserContext _context = null!;
    private bool _trace;
    protected IPage Page { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task PageBaseSetUp()
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Configuration.GetValue<bool>("Playwright:Headless", true),
            SlowMo = Configuration.GetValue<float?>("Playwright:SlowMoMilliseconds", null),
        });
        _context = await browser.NewContextAsync();

        _trace = Configuration.GetValue("Playwright:Tracing:Enabled", false);
        if (_trace)
        {
            // Start tracing before creating / navigating a page.
            await _context.Tracing.StartAsync(new()
            {
                Screenshots = Configuration.GetValue("Playwright:Tracing:Screenshots", true),
                Snapshots = Configuration.GetValue("Playwright:Tracing:Snapshots", true),
                Sources = Configuration.GetValue("Playwright:Tracing:Sources", true)
            });
        }

        var page = await _context.NewPageAsync();
        _browser = browser;
        _pw = playwright;
        Page = page;
    }

    [OneTimeTearDown]
    public async Task BaseTearDown()
    {
        if (_trace)
        {
            // Stop tracing and export it into a zip archive.
            await _context.Tracing.StopAsync(new()
            {
                Path = "trace.zip"
            });
        }

        await _browser!.DisposeAsync();
        _pw!.Dispose();
    }
}