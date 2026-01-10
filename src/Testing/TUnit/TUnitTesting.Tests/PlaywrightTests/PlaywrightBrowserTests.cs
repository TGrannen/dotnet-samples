using TUnit.Playwright;

namespace TUnitTesting.Tests.PlaywrightTests;

/// <summary>
/// Example tests showing how to use different browsers with TUnit.Playwright
/// Override the BrowserName property to control which browser is launched
/// </summary>
public class PlaywrightChromiumTests : PageTest
{
    /// <summary>
    /// Default browser is Chromium, but you can explicitly override it
    /// </summary>
    public override string BrowserName => "chromium";
    
    [Test]
    public async Task TestWithChromium()
    {
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        await Assert.That(Page.Url).Contains("github.com");
    }
}

public class PlaywrightFirefoxTests : PageTest
{
    public override string BrowserName => "firefox";
    
    [Test]
    public async Task TestWithFirefox()
    {
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        await Assert.That(Page.Url).Contains("github.com");
    }
}

public class PlaywrightWebKitTests : PageTest
{
    public override string BrowserName => "webkit";
    
    [Test]
    public async Task TestWithWebKit()
    {
        await Page.GotoAsync("https://www.github.com/thomhurst/TUnit");
        await Assert.That(Page.Url).Contains("github.com");
    }
}