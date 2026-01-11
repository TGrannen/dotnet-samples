using System.Text.Json;

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public static class PlaywrightExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<bool> HasValidAuthStateAsync(this string authStatePath)
    {
        var authStateDirectory = Path.GetDirectoryName(authStatePath);
        if (!Directory.Exists(authStateDirectory))
        {
            Directory.CreateDirectory(authStateDirectory!);
        }

        if (!File.Exists(authStatePath) || new FileInfo(authStatePath).Length <= 0)
        {
            return false;
        }

        var json = await File.ReadAllTextAsync(authStatePath);
        var authState = JsonSerializer.Deserialize<AuthState>(json, JsonSerializerOptions);
        return authState is { Cookies.Length: > 0 };
    }

    public static async Task PerformLoginAsync(this IPage page, AuthOptions authOptions)
    {
        if (string.IsNullOrEmpty(authOptions.Email) || string.IsNullOrEmpty(authOptions.Password))
        {
            throw new InvalidOperationException(
                "Authentication credentials are not configured. Please set 'Authentication:Email' and 'Authentication:Password' in user secrets.");
        }

        await page.GotoAsync("https://guillotineleagues.com/", new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Log In" }).First.ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).FillAsync(authOptions.Email);
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email" }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password" }).FillAsync(authOptions.Password);
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" }).ClickAsync(new LocatorClickOptions { Timeout = 30_000 });
        await Expect(page).ToHaveURLAsync("https://guillotineleagues.com/lobby/public", new PageAssertionsToHaveURLOptions
        {
            IgnoreCase = true,
            Timeout = 30_000
        });
    }

    public class AuthState
    {
        public required CookiesModel[] Cookies { get; set; }
        public required OriginsModel[] Origins { get; set; }

        public class CookiesModel
        {
            public required string Name { get; set; }
            public required string Value { get; set; }
            public required string Domain { get; set; }
            public required string Path { get; set; }
            public double Expires { get; set; }
            public bool HttpOnly { get; set; }
            public bool Secure { get; set; }
            public required string SameSite { get; set; }
        }

        public class OriginsModel
        {
            public string Origin { get; set; } = null!;
            public LocalStorageModel[]? LocalStorage { get; set; }
        }

        public class LocalStorageModel
        {
            public required string Name { get; set; }
            public required string Value { get; set; }
        }
    }
}