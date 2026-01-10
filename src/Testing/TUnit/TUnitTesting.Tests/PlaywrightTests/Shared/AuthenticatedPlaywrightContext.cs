using Microsoft.Extensions.Options;

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public class AuthenticatedPlaywrightContext : IAsyncInitializer
{
    [ClassDataSource<ConfigurationContext>(Shared = SharedType.PerTestSession)]
    public required ConfigurationContext ConfigurationContext { get; set; } = null!;

    private static readonly string AuthStatePath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "playwright", ".auth", "state.json"));

    public string AuthenticationStatePath => AuthStatePath;

    public async Task InitializeAsync()
    {
        // Ensure the playwright/.auth directory exists
        var authDirectory = Path.GetDirectoryName(AuthStatePath);
        if (!string.IsNullOrEmpty(authDirectory) && !Directory.Exists(authDirectory))
        {
            Directory.CreateDirectory(authDirectory);
        }

        // Only perform login if authentication state doesn't exist or is invalid
        if (File.Exists(AuthStatePath) && new FileInfo(AuthStatePath).Length > 0)
        {
            return;
        }

        // Launch Playwright browser for authentication
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 200
        });

        var context = await browser.NewContextAsync();
        var loginPage = await context.NewPageAsync();
        await PerformLoginAsync(loginPage);
        await context.StorageStateAsync(new BrowserContextStorageStateOptions
        {
            Path = AuthStatePath
        });

        await context.CloseAsync();
        await browser.CloseAsync();
    }

    private async Task PerformLoginAsync(IPage page)
    {
        var authOptions = ConfigurationContext.GetOptions<AuthOptions>();

        if (string.IsNullOrEmpty(authOptions.Email) || string.IsNullOrEmpty(authOptions.Password))
        {
            throw new InvalidOperationException(
                "Authentication credentials are not configured. Please set 'Authentication:Email' and 'Authentication:Password' in user secrets.");
        }

        await page.GotoAsync("https://guillotineleagues.com/");
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Log In" }).First.ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).FillAsync(authOptions.Email);
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password" }).FillAsync(authOptions.Password);
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" }).ClickAsync();
    }
}