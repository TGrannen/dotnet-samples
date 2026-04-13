using ApiService.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TUnit.AspNetCore;

namespace ApiService.Api.Tests.Shared;

public class WebApplicationFactory : TestWebApplicationFactory<Program>
{
    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    public PostgresContainer Postgres { get; init; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // Must override appsettings; in-memory config from ConfigureStartupConfiguration can lose to JSON.
        builder.UseSetting("ConnectionStrings:DefaultConnection", Postgres.GetConnectionString());
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        return host;
    }

    public async Task SeedAsync(Func<ApplicationDbContext, Task> seedAction)
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await seedAction(db);
        await db.SaveChangesAsync();
    }
}
