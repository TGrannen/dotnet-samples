namespace PlaywrightTests.Setup;

public abstract class WeatherUndergroundAuthenticatedPageBase : PageBase
{
    [OneTimeSetUp]
    public async Task AuthenticatedPageBaseSetUp()
    {
        var url = Configuration.GetValue<string>("WeatherUnderground:Url");
        await Page.GotoAsync(url);
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Log in" }).ClickAsync();
        await Page.WaitForURLAsync("**/login");

        var email = Configuration.GetValue<string>("WeatherUnderground:EmailAddress");
        var password = Configuration.GetValue<string>("WeatherUnderground:Password");
        await Page.GetByLabel("Email").FillAsync(email);
        await Page.GetByLabel("Password").FillAsync(password);
        await Page.GetByLabel("Sign In").ClickAsync();

        await Page.WaitForURLAsync(url);
    }
}