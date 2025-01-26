using Npgsql;
using Respawn;

namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests;

[TestFixture]
public abstract class DbRespawnTests
{
    [SetUp]
    public async Task SetUp()
    {
        await using var conn = new NpgsqlConnection(DatabaseSetupFixtureTestData.ConnectionString);
        await conn.OpenAsync();
        var respawner = Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        }).GetAwaiter().GetResult();
        await respawner.ResetAsync(conn);
    }
}