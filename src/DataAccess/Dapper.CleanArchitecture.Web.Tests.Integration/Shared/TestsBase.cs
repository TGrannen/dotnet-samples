using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared;

public abstract class TestsBase : IAsyncDisposable
{
    private readonly AppFactory _factory;
    private readonly Respawner _respawner;

    protected TestsBase(AppFactory factory)
    {
        _factory = factory;
        var connectionString = ConnectionString();
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        _respawner = Respawner.CreateAsync(conn, new RespawnerOptions
        {
            SchemasToInclude =
            [
                "public"
            ],
            DbAdapter = DbAdapter.Postgres
        }).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        var logger = _factory.Services.GetRequiredService<ILogger<TestsBase>>();
        var connectionString = ConnectionString();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
        logger.LogInformation("Wiping Database");
        await _factory.DisposeAsync();
    }

    private string ConnectionString()
    {
        var connectionStringProvider = _factory.Services.GetRequiredService(typeof(IDbConnectionStringProvider)) as IDbConnectionStringProvider;
        var connectionString = connectionStringProvider!.GetConnectionString();
        return connectionString;
    }
}