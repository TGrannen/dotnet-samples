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
}
