using TUnit.Playwright;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public abstract class PageTestBase : PlaywrightTest
{
    private static IBrowser? _browser;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static IBrowserContext? _sharedContext;
    private IBrowserContext? _testContext;
    private PlaywrightOptions _playwrightOptions = null!;

    [ClassDataSource<ConfigurationContext>(Shared = SharedType.PerAssembly)]
    public required ConfigurationContext Config { get; set; } = null!;

    public IPage Page { get; private set; } = null!;

    /// <summary>
    /// Override this method to customize the browser context options.
    /// </summary>
    /// <returns>BrowserNewContextOptions to use when creating contexts</returns>
    protected virtual BrowserNewContextOptions GetBrowserNewContextOptions()
    {
        return new BrowserNewContextOptions
        {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light
        };
    }

    [Before(Test)]
    public async Task ContextSetup()
    {
        _playwrightOptions = Config.GetOptions<PlaywrightOptions>();
        await Semaphore.WaitAsync();
        var reusingContext = _playwrightOptions.ReuseContext;

        try
        {
            _browser ??= await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = !_playwrightOptions.Headed,
                SlowMo = (float?)_playwrightOptions.SlowMo?.TotalMilliseconds,
            });

            var contextOptions = GetBrowserNewContextOptions();

            if (reusingContext)
            {
                // Reuse shared context
                _sharedContext ??= await _browser.NewContextAsync(contextOptions).ConfigureAwait(false);
                _testContext = null; // No per-test context when reusing
            }
            else
            {
                // Create a new context for each test
                _testContext = await _browser.NewContextAsync(contextOptions).ConfigureAwait(false);
            }
        }
        finally
        {
            Semaphore.Release();
        }

        var contextToUse = reusingContext ? _sharedContext! : _testContext!;
        Page = await contextToUse.NewPageAsync().ConfigureAwait(false);
    }

    [After(Test)]
    public async Task Teardown()
    {
        await Page.CloseAsync();

        if (!_playwrightOptions.ReuseContext && _testContext != null)
        {
            await _testContext.CloseAsync().ConfigureAwait(false);
            _testContext = null;
        }
    }
}