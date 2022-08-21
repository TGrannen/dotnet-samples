using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared.Fixtures;

public class PostgresDbFixture : IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _container;
    public PostgresDbFixture()
    {
        var containersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "dapperdb",
                Username = "postgres",
                Password = "test-password",
            })
            .WithName("postgres-dotnet-samples");

        _container = containersBuilder.Build();
    }

    public PostgreSqlTestcontainer Container => _container;

    public async Task DisposeAsync()
    {
        await _container?.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }
}