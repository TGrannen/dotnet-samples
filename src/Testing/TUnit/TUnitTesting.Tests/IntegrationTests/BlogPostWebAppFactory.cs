using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TUnitTesting.Tests.IntegrationTests.BlogPosts;
using TUnitTesting.WebApi.Data;

namespace TUnitTesting.Tests.IntegrationTests;

public class BlogPostWebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithPassword("postgres")
        .WithUsername("postgres")
        .WithDatabase("test_db")
        .Build();

    private Respawner? _respawner;
    private string? _connectionString;

    public IBlogPostCommentApi CommentAPI { get; set; } = null!;

    public IBlogPostApi PostAPI { get; set; } = null!;

    public async Task InitializeAsync()
    {
        // You can also override certain services here to mock things out
        // Start the container
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();

        // Grab a reference to the server
        // This forces it to initialize.
        // By doing it within this method, it's thread safe.
        // And avoids multiple initialisations from different tests if parallelisation is switched on
        _ = Server;

        await EnsureDatabaseCreatedAsync();

        await SetupRespawner();

        PostAPI = Refit.RestService.For<IBlogPostApi>(CreateClient());
        CommentAPI = Refit.RestService.For<IBlogPostCommentApi>(CreateClient());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_connectionString));
        });
    }

    private async Task EnsureDatabaseCreatedAsync()
    {
        using var scope = Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task SetupRespawner()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
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

    public async Task ResetBlogPostsAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await _respawner!.ResetAsync(conn!);
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}