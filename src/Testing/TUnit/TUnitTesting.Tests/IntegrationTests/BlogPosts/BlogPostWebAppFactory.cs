using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using TUnitTesting.WebApi.Data;

namespace TUnitTesting.Tests.IntegrationTests.BlogPosts;

public abstract class TestsBase : WebApplicationTest<BlogPostWebAppFactory, Program>
{
    private static Respawner? _respawner;
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    private PostgresContainer Postgres { get; init; } = null!;

    protected IBlogPostCommentApi CommentAPI { get; set; } = null!;

    protected IBlogPostApi PostAPI { get; set; } = null!;

    [Before(Test)]
    public async Task BeforeTest()
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            using var scope = Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            if ((await db.Database.GetPendingMigrationsAsync()).Any())
            {
                await db.Database.EnsureCreatedAsync();
            }
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        PostAPI = Refit.RestService.For<IBlogPostApi>(Factory.CreateClient());
        CommentAPI = Refit.RestService.For<IBlogPostCommentApi>(Factory.CreateClient());
    }

    public async Task ResetBlogPostsAsync()
    {
        await SetupRespawner();
        await using var conn = new NpgsqlConnection(Postgres.Container.GetConnectionString());
        await conn.OpenAsync();
        await _respawner!.ResetAsync(conn);
    }

    private async Task SetupRespawner()
    {
        if (_respawner != null)
        {
            return;
        }

        await using var conn = new NpgsqlConnection(Postgres.Container.GetConnectionString());
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

public class BlogPostWebAppFactory : TestWebApplicationFactory<Program>
{
    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    public PostgresContainer Postgres { get; init; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Postgres.Container.GetConnectionString()));
        });
    }
}