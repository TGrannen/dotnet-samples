using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TUnitTesting.WebApi.Data;

namespace TUnitTesting.Tests.IntegrationTests.BlogPosts.Shared;

public class PostgresContainer : IAsyncInitializer, IAsyncDisposable
{
    private Respawner? _respawner;

    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("postgres:latest")
        .WithPassword("postgres")
        .WithUsername("postgres")
        .WithDatabase("test_db")
        .Build();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        await InitializeDatabase();

        await SetupRespawner();
    }

    public async ValueTask DisposeAsync() => await Container.DisposeAsync();

    public async ValueTask Respawn()
    {
        await using var conn = new NpgsqlConnection(Container.GetConnectionString());
        await conn.OpenAsync();
        await _respawner!.ResetAsync(conn);
    }

    private async Task InitializeDatabase()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Container.GetConnectionString()));
        using var scope = services.BuildServiceProvider().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    private async Task SetupRespawner()
    {
        await using var conn = new NpgsqlConnection(Container.GetConnectionString());
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToInclude =
            [
                "Posts",
                "Comments",
            ]
        });
    }
}