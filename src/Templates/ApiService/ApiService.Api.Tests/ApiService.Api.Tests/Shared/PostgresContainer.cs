using ApiService.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        await PerformDatabaseMigrationsAsync();
    }

    private ServiceProvider InitializeServiceProvider()
    {
        var serviceCollection = new ServiceCollection();
        var configuration =
            new ConfigurationManager().AddInMemoryCollection(new List<KeyValuePair<string, string?>>
            {
                new("ConnectionStrings:DefaultConnection", _container.GetConnectionString())
            });
        serviceCollection.AddPersistence(configuration.Build());
        var provider = serviceCollection.BuildServiceProvider();
        return provider;
    }

    private async Task PerformDatabaseMigrationsAsync()
    {
        var provider = InitializeServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var migrations = await db.Database.GetPendingMigrationsAsync();
        if (migrations.Any())
        {
            await db.Database.MigrateAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
