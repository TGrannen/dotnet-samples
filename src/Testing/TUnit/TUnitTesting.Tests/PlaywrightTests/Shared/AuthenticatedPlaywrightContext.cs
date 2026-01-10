using System.Text.Json;

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

#pragma warning disable CS8618
public class AuthenticatedPlaywrightContext : IAsyncInitializer
{
    [ClassDataSource<ConfigurationContext>(Shared = SharedType.PerAssembly)]
    public required ConfigurationContext ConfigurationContext { get; set; } = null!;

    private static readonly string AuthStateDirectory =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "playwright", ".auth"));

    private static readonly string AuthStatePath = Path.GetFullPath(Path.Combine(AuthStateDirectory, "state.json"));

    public string AuthenticationStatePath => AuthStatePath;

    public async Task InitializeAsync()
    {
        if (!Directory.Exists(AuthStateDirectory))
        {
            Directory.CreateDirectory(AuthStateDirectory);
        }

        // Only perform login if authentication state doesn't exist or is invalid
        if (File.Exists(AuthStatePath) && new FileInfo(AuthStatePath).Length > 0)
        {
            var json = await File.ReadAllTextAsync(AuthStatePath);
            var authState = JsonSerializer.Deserialize<AuthState>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (authState is { Cookies.Length: > 0 })
            {
                return;
            }
        }

        var playwrightOptions = ConfigurationContext.GetOptions<PlaywrightOptions>();

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = !playwrightOptions.Headed,
            SlowMo = 200
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

    public class AuthState
    {
        public CookiesModel[] Cookies { get; set; }
        public OriginsModel[] Origins { get; set; }

        public class CookiesModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Domain { get; set; }
            public string Path { get; set; }
            public double Expires { get; set; }
            public bool HttpOnly { get; set; }
            public bool Secure { get; set; }
            public string SameSite { get; set; }
        }

        public class OriginsModel
        {
            public string Origin { get; set; }
            public LocalStorage[] LocalStorage { get; set; }
        }

        public class LocalStorage
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}