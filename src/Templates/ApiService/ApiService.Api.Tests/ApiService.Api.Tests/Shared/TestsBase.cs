using ApiService.Api.Persistence;
using Microsoft.Extensions.DependencyInjection;
using TUnit.AspNetCore;

namespace ApiService.Api.Tests.Shared;

public abstract class IntegrationTestsBase : WebApplicationTest<WebApplicationFactory, Program>
{
    protected async Task SeedAsync(Func<ApplicationDbContext, Task> seedAction)
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await seedAction(db);
        await db.SaveChangesAsync();
    }

    protected void SaveChangesTrackerReset()
    {
        var tracker = Services.GetRequiredService<SaveChangesTracker>();
        tracker.Count = 0;
    }

    /// Executes a set of assertions after each test execution to validate the consistency of the system.
    /// Specifically, checks that the `SaveChanges` operation during the test execution
    /// has occurred no more than once by inspecting the `SaveChangesTracker` service.
    /// This method is annotated with the `[After(Test)]` attribute, ensuring it runs automatically
    /// after each test in the test class.
    [After(Test)]
    public async Task AfterTestAsync()
    {
        var tracker = Services.GetRequiredService<SaveChangesTracker>();
        await Assert.That(tracker.Count).IsLessThanOrEqualTo(1);
    }
}
