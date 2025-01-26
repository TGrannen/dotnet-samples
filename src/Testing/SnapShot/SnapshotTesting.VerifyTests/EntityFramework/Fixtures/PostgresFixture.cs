using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Respawn;

namespace SnapshotTesting.VerifyTests.EntityFramework.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    public readonly PostgreSqlTestcontainer Container;

    public static readonly Checkpoint Checkpoint = new()
    {
        SchemasToInclude = new[]
        {
            "public"
        },
        DbAdapter = DbAdapter.Postgres
    };

    public PostgresFixture()
    {
        Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "test_db",
                Username = "postgres",
                Password = "postgres",
            })
            .WithImage("postgres:11")
            .WithCleanUp(true)
            .Build();
    }


    public async Task InitializeAsync() => await Container.StartAsync();

    public async Task DisposeAsync() => await Container.DisposeAsync();
}