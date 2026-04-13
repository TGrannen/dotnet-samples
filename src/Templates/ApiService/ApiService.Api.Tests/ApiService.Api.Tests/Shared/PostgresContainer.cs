using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace ApiService.Api.Tests.Shared;

/// <summary>Ephemeral PostgreSQL for integration tests (requires Docker).</summary>
public sealed class PostgresContainer : IAsyncInitializer, IAsyncDisposable
{
    private PostgreSqlContainer? _container;

    public string GetConnectionString() =>
        _container?.GetConnectionString() ?? throw new InvalidOperationException("PostgreSQL container is not started.");

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("apiservice_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
