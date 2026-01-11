namespace TUnitTesting.Tests.PlaywrightTests.Shared;

#pragma warning disable CS8618
public class AuthenticatedPlaywrightContext : IAsyncInitializer
{
    [ClassDataSource<ConfigurationContext>(Shared = SharedType.PerAssembly)]
    public required ConfigurationContext ConfigurationContext { get; set; } = null!;

    private static readonly string AuthStateDirectory =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "playwright", "auth"));

    private static readonly string AuthStatePath = Path.GetFullPath(Path.Combine(AuthStateDirectory, "state.json"));

    public string AuthenticationStatePath => AuthStatePath;

    public async Task InitializeAsync()
    {
        if (await AuthStatePath.HasValidAuthStateAsync())
        {
            return;
        }

        var playwrightOptions = ConfigurationContext.GetOptions<PlaywrightOptions>();

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = !playwrightOptions.Headed,
            SlowMo = (float?)playwrightOptions.SlowMo?.TotalMilliseconds
        });

        var context = await browser.NewContextAsync();

        var loginPage = await context.NewPageAsync();

        var authOptions = ConfigurationContext.GetOptions<AuthOptions>();
        await loginPage.PerformLoginAsync(authOptions);

        await context.StorageStateAsync(new BrowserContextStorageStateOptions
        {
            Path = AuthStatePath
        });

        await loginPage.CloseAsync();
        await context.CloseAsync();
        await browser.DisposeAsync();
    }
}