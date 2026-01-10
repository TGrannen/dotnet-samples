using Testcontainers.PostgreSql;

namespace TUnitTesting.Tests.IntegrationTests.BlogPosts;

public class PostgresContainer : IAsyncInitializer, IAsyncDisposable
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("postgres:latest")
        .WithPassword("postgres")
        .WithUsername("postgres")
        .WithDatabase("test_db")
        .Build();

    public async Task InitializeAsync() => await Container.StartAsync();
    public async ValueTask DisposeAsync() => await Container.DisposeAsync();
}