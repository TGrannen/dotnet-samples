using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared;

public abstract class TestsBase : IDisposable
{
    private readonly AppFactory _factory;
    private static Checkpoint _checkpoint = null!;

    protected TestsBase(AppFactory factory)
    {
        _factory = factory;
        _checkpoint ??= new Checkpoint
        {
            DbAdapter = DbAdapter.Postgres
        };
    }

    public void Dispose()
    {
        var logger =_factory.Services.GetRequiredService<ILogger<TestsBase>>();
        var connectionStringProvider =_factory.Services.GetRequiredService(typeof(IDbConnectionStringProvider)) as IDbConnectionStringProvider;
        var connectionString = connectionStringProvider!.GetConnectionString();
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        logger.LogInformation("Wiping Database");
        _checkpoint.Reset(conn).Wait();
    }
}