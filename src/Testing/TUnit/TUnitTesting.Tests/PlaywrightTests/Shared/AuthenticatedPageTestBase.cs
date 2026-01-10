using TUnit.Playwright;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

[ClassDataSource<ConfigurationContext>(Shared = SharedType.PerAssembly)]
public abstract class AuthenticatedPageTestBase(ConfigurationContext config) : PageTest(new BrowserTypeLaunchOptions
{
    Headless = !config.GetOptions<PlaywrightOptions>().Headed,
    SlowMo = (float?)config.GetOptions<PlaywrightOptions>().SlowMo?.TotalMilliseconds,
})
{
    [ClassDataSource<AuthenticatedPlaywrightContext>(Shared = SharedType.PerTestSession)]
    public required AuthenticatedPlaywrightContext AuthContext { get; set; } = null!;

    public override BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        return new BrowserNewContextOptions
        {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light,
            StorageStatePath = AuthContext.AuthenticationStatePath
        };
    }
}