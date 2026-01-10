using TUnitTesting.Tests.PlaywrightTests.Shared;

namespace TUnitTesting.Tests.PlaywrightTests;

public class AuthenticatedNavigationTests(ConfigurationContext config) : AuthenticatedPageTestBase(config)
{
    [Test]
    public async Task NavigateToAuthenticatedPage(CancellationToken cancellationToken)
    {
        await Page.GotoAsync("https://guillotineleagues.com/lobby/public");
        await Expect(Page).ToHaveURLAsync("https://guillotineleagues.com/lobby/public");
    }

    [Test]
    public async Task CheckAuthenticatedUserProfile(CancellationToken cancellationToken)
    {
        await Page.GotoAsync("https://guillotineleagues.com/account/profile/about");
        await Expect(Page).ToHaveURLAsync("https://guillotineleagues.com/account/profile/about");
    }

    [Test]
    public async Task FullTestNavigation(CancellationToken cancellationToken)
    {
        await Page.GotoAsync("https://guillotineleagues.com/");
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "My Teams" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Chopped" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "My Drafts" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "NOCO pain, no gain Free to" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Teams", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "League Rosters" }).ClickAsync();
    }
}