namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public static class PlaywrightExtensions
{
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
}