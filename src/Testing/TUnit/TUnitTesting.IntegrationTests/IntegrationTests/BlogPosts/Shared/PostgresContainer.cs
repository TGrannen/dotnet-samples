using System.Security.Cryptography;
using System.Text;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TUnitTesting.WebApi.Data;

namespace TUnitTesting.IntegrationTests.IntegrationTests.BlogPosts.Shared;

public class PostgresContainer : IAsyncInitializer, IAsyncDisposable
{
    private Respawner? _respawner;

    private readonly PostgreSqlBuilder _postgreSqlBuilder = new PostgreSqlBuilder("postgres:latest")
        .WithPassword("postgres")
        .WithUsername("postgres")
        .WithDatabase("test_db")
        .WithReuse(true);

    public PostgreSqlContainer Container { get; private set; }

    public async Task InitializeAsync()
    {
        var migrationIdentity = await GetMigrationIdentity();

        Container = _postgreSqlBuilder
            .WithName($"integration-tests-{migrationIdentity}")
            .Build();

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

    private async Task<string> GetMigrationIdentity()
    {
        return await DatabaseAction<string>(db =>
        {
            // Create a unique string based on migration names
            var migrations = db.Database.GetMigrations().ToArray();

            var identityString = string.Join("|", migrations);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(identityString));
            var shortHash = Convert.ToHexString(hash)[..8];

            // Format: LastMigrationName-TotalCount-Hash
            return Task.FromResult<string>($"{migrations.Last()}_Count_{migrations.Length}-{shortHash.ToLower()}");
        });
    }

    private async Task InitializeDatabase()
    {
        await DatabaseAction<string>(async db =>
        {
            await db.Database.MigrateAsync();
            return string.Empty;
        });
    }

    private async Task<T> DatabaseAction<T>(Func<ApplicationDbContext, Task<T>> func)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Container?.GetConnectionString() ?? "postgres"));
        using var scope = services.BuildServiceProvider().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await func(db);
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