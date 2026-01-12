using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace SnapshotTesting.VerifyTests.EntityFramework.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer Container = new PostgreSqlBuilder("postgres:11")
        .WithDatabase("test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithImage("postgres:11")
        .WithCleanUp(true)
        .Build();

    private Respawner? _respawner;

    public async Task<Respawner> GetRespawner()
    {
        if (_respawner != null)
        {
            return _respawner;
        }

        await using var connection = new NpgsqlConnection(Container.GetConnectionString());
        await connection.OpenAsync();
        return _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            SchemasToInclude =
            [
                "public"
            ],
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        await using var connection = new NpgsqlConnection(Container.GetConnectionString());
        await connection.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}