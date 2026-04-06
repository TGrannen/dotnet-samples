using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TUnitTesting.IntegrationTests.IntegrationTests.BlogPosts.Shared;

public class BlogPostWebAppFactory : TestWebApplicationFactory<WebApi.Program>
{
    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    public PostgresContainer Postgres { get; init; } = null!;

    protected override void ConfigureStartupConfiguration(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", Postgres.Container.GetConnectionString() }
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
