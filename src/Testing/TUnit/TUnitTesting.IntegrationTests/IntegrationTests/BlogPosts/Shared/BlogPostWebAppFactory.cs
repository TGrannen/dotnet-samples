using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TUnitTesting.WebApi.Data;

namespace TUnitTesting.IntegrationTests.IntegrationTests.BlogPosts.Shared;

public class BlogPostWebAppFactory : TestWebApplicationFactory<WebApi.Program>
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
