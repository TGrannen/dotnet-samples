namespace PlaywrightTests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class WeatherUndergroundProfileTests : WeatherUndergroundAuthenticatedPageBase
{
    [Test]
    public async Task MyProfileButton_ShouldBeVisible_WhenUserIsLoggedIn()
    {
        var myProfile = Page.GetByRole(AriaRole.Button, new() { NameString = "My Profile" });
        await Expect(myProfile).ToBeVisibleAsync();
    }
}