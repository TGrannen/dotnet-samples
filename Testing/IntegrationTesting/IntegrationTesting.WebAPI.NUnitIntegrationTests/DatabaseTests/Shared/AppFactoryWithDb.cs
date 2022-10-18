using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests;

public class AppFactoryWithDb<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DbConnectionString", DatabaseSetupFixtureTestData.ConnectionString },
            });
        });

        return base.CreateHost(builder);
    }
}