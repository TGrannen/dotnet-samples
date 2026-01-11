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
    private string? _traceFilePath;

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
        var tracingOptions = _playwrightOptions.Tracing;
        reusingContext = reusingContext && !tracingOptions.Enabled;

        try
        {
            _browser ??= await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = !_playwrightOptions.Headed,
                SlowMo = (float?)_playwrightOptions.SlowMo?.TotalMilliseconds,
            });

            if (reusingContext)
            {
                // Reuse shared context
                _sharedContext ??= await _browser.NewContextAsync(GetBrowserNewContextOptions()).ConfigureAwait(false);
                _testContext = _sharedContext;
            }
            else
            {
                // Create a new context for each test
                _testContext = await _browser.NewContextAsync(GetBrowserNewContextOptions()).ConfigureAwait(false);
            }
        }
        finally
        {
            Semaphore.Release();
        }

        // Start tracing if enabled
        if (tracingOptions.Enabled)
        {
            await StartTracing(tracingOptions);
        }

        Page = await _testContext.NewPageAsync().ConfigureAwait(false);
    }

    [After(Test)]
    public async Task Teardown()
    {
        await Page.CloseAsync();

        if (_playwrightOptions.Tracing.Enabled && _testContext != null)
        {
            await _testContext.Tracing.StopAsync(new TracingStopOptions
            {
                Path = _traceFilePath
            });
        }

        if (!_playwrightOptions.ReuseContext && _testContext != null)
        {
            await _testContext.CloseAsync().ConfigureAwait(false);
            _testContext = null;
        }
    }

    private async Task StartTracing(TracingOptions tracingOptions)
    {
        var testClass = TestContext.Current!.ClassContext.ClassType.Name;
        var testMethod = TestContext.Current!.Metadata.TestName;
        var title = $"{testClass}.{testMethod}";
        var traceFileName = $"{title}.zip";

        var outputDir = tracingOptions.OutputDirectory ?? "traces";
        var outputPath = Path.Combine(AppContext.BaseDirectory, outputDir);
        Directory.CreateDirectory(outputPath);

        _traceFilePath = Path.Combine(outputPath, traceFileName);

        await _testContext!.Tracing.StartAsync(new TracingStartOptions
        {
            Title = title,
            Screenshots = tracingOptions.Screenshots,
            Snapshots = tracingOptions.Snapshots,
            Sources = tracingOptions.Sources
        });
    }
}