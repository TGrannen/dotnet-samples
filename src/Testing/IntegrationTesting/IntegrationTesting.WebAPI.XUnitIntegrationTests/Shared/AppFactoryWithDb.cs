using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IntegrationTesting.WebAPI.XUnitIntegrationTests.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

public class AppFactoryWithDb<TStartup> : AppFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    public bool ShouldSeed { get; set; }

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("integrationTestDb")
        .WithUsername("postgres")
        .WithPassword("test-password")
        .Build();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
            {
                new("ConnectionStrings:DbConnectionString", _container.GetConnectionString()),
            });
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await DbSeeder.CreateSchema(_container.GetConnectionString());
        if (ShouldSeed)
        {
            await DbSeeder.SeedData(_container.GetConnectionString());
        }
    }

    public new async Task DisposeAsync()
    {
        await _container.StopAsync()!;
    }
}

public class AppFactoryWithSeededDb<TStartup> : AppFactoryWithDb<TStartup>, IAsyncLifetime where TStartup : class
{
    public AppFactoryWithSeededDb()
    {
        ShouldSeed = true;
    }
}