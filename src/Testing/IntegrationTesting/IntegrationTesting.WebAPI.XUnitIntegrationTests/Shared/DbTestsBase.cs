using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

public abstract class DbTestsBase<T> : IDisposable where T : class
{
    private readonly AppFactoryWithDb<T> _factory;

    protected DbTestsBase(AppFactoryWithDb<T> factory)
    {
        _factory = factory;
    }

    public void Dispose()
    {
        var logger = _factory.Services.GetRequiredService<ILogger<DbTestsBase<T>>>();
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        logger.LogInformation("Wiping Database");
        using var conn = new NpgsqlConnection(configuration.GetValue<string>("ConnectionStrings:DbConnectionString"));
        conn.OpenAsync().Wait();
        var respawner = Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        }).GetAwaiter().GetResult();
        respawner.ResetAsync(conn).Wait();
    }
}