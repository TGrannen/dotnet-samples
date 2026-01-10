using TUnit.Playwright;

namespace TUnitTesting.Tests.PlaywrightTests;

/// <summary>
/// Example showing how to access Playwright objects directly
/// Available properties: Page, Context, Browser, Playwright
/// </summary>
public class PlaywrightAdvancedTests : PageTest
{
    [Test]
    public async Task AccessPlaywrightObjects()
    {
        // Access the Page object
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Access the BrowserContext
        var contextCookies = await Context.CookiesAsync();
        await Assert.That(contextCookies).IsNotNull();
        
        // Access the Browser object
        var browserVersion = Browser.Version;
        await Assert.That(browserVersion).IsNotNull();
        
        // Access the Playwright object
        await Assert.That(Playwright).IsNotNull();
    }
    
    [Test]
    public async Task CreateNewPage()
    {
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Create a new page in the same context
        var newPage = await Context.NewPageAsync();
        await newPage.GotoAsync("https://www.github.com");
        
        await Assert.That(newPage.Url).Contains("github.com");
        
        // Clean up the new page
        await newPage.CloseAsync();
    }
}