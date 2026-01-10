#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

[ParallelLimiter<AuthenticatedParallelLimit>]
public abstract class AuthenticatedPageTestBase : PageTestBase
{
    [ClassDataSource<AuthenticatedPlaywrightContext>(Shared = SharedType.PerTestSession)]
    public required AuthenticatedPlaywrightContext AuthContext { get; set; } = null!;

    protected override BrowserNewContextOptions GetBrowserNewContextOptions()
    {
        var baseOptions = base.GetBrowserNewContextOptions();
        baseOptions.StorageStatePath = AuthContext.AuthenticationStatePath;
        return baseOptions;
    }

    public record AuthenticatedParallelLimit : IParallelLimit
    {
        public int Limit => 5;
    }
}