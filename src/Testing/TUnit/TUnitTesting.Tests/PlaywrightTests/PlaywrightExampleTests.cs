using TUnit.Playwright;

namespace TUnitTesting.Tests.PlaywrightTests;

/// <summary>
/// Example Playwright tests using TUnit.Playwright
/// These tests inherit from PageTest which handles setup and disposal of Playwright objects
/// </summary>
public class PlaywrightExampleTests : PageTest
{
    [Test]
    public async Task NavigateToTUnitGitHub()
    {
        // Navigate to the TUnit GitHub repository
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Assert that we're on the correct page
        await Assert.That(Page.Url).Contains("thomhurst/TUnit");
    }
    
    [Test]
    public async Task CheckPageTitle()
    {
        // Navigate to a website
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Wait for the page to load and check the title
        var title = await Page.TitleAsync();
        
        await Assert.That(title).IsNotNull();
        await Assert.That(title).Contains("TUnit");
    }
    
    [Test]
    public async Task InteractWithPageElements()
    {
        // Navigate to GitHub
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Wait for page to load
        await Page.WaitForLoadStateAsync();
        
        // Check that we can access page elements
        var heading = await Page.Locator("h1").First.InnerTextAsync();
        
        await Assert.That(heading).IsNotNull();
    }
    
    [Test]
    public async Task TakeScreenshot()
    {
        // Navigate to a page
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        
        // Wait for the page to fully load
        await Page.WaitForLoadStateAsync();
        
        // Take a screenshot (this will be saved automatically by Playwright)
        await Page.ScreenshotAsync(new Microsoft.Playwright.PageScreenshotOptions
        {
            Path = "screenshot.png"
        });
        
        // Verify the page loaded successfully
        await Assert.That(Page.Url).IsNotNull();
    }
}

