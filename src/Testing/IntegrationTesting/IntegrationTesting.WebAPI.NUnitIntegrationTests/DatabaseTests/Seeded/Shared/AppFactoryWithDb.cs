using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests.Seeded;

public class AppFactoryWithSeededDb<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:DbConnectionString", SeededDatabaseSetupFixtureTestData.ConnectionString
                },
            });
        });

        return base.CreateHost(builder);
    }
}