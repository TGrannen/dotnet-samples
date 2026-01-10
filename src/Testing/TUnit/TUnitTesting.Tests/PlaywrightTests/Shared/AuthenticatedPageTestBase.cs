using TUnit.Playwright;

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public abstract class AuthenticatedPageTestBase : PageTest
{
    [ClassDataSource<AuthenticatedPlaywrightContext>(Shared = SharedType.PerTestSession)]
    public required AuthenticatedPlaywrightContext AuthContext { get; set; } = null!;
    
    [Before(Test)]
    public async Task ApplyAuthenticationState()
    {
        await Context.StorageStateAsync(new BrowserContextStorageStateOptions
        {
            Path = AuthContext.AuthenticationStatePath
        });
    }
}