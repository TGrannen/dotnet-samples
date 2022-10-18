using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IntegrationTesting.WebAPI.XUnitIntegrationTests.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

public class AppFactoryWithDb<TStartup> : AppFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    private readonly PostgreSqlTestcontainer _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "integrationTestDb",
            Username = "postgres",
            Password = "test-password",
        }).Build();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DbConnectionString", _container.ConnectionString },
            });
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await DbSeeder.CreateSchema(_container.ConnectionString);
        await DbSeeder.SeedData(_container.ConnectionString);
    }

    public new async Task DisposeAsync()
    {
        await _container?.StopAsync()!;
    }
}