using Microsoft.AspNetCore.Hosting;
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
}
