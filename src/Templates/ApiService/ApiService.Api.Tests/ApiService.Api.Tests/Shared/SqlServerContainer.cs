using ApiService.Api.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using TUnit.Core.Interfaces;

namespace ApiService.Api.Tests.Shared;

/// <summary>Ephemeral SQL Server for integration tests (requires Docker).</summary>
public sealed class SqlServerContainer : IAsyncInitializer, IAsyncDisposable
{
    // private const string TestDatabaseName = "apiservice_tests";

    private MsSqlContainer? _container;

    public string GetConnectionString() =>
        _container is not null
            ? new SqlConnectionStringBuilder(_container.GetConnectionString()).ConnectionString
            : throw new InvalidOperationException("SQL Server container is not started.");

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123!")
            .Build();

        await _container.StartAsync();

        // await EnsureTestDatabaseExistsAsync();
        await PerformDatabaseMigrationsAsync();
    }

    // private async Task EnsureTestDatabaseExistsAsync()
    // {
    //     var masterCs = new SqlConnectionStringBuilder(_container!.GetConnectionString())
    //     {
    //         InitialCatalog = "master"
    //     }.ConnectionString;
    //
    //     await using var conn = new SqlConnection(masterCs);
    //     await conn.OpenAsync();
    //     await using var cmd = conn.CreateCommand();
    //     cmd.CommandText = $"IF DB_ID(N'{TestDatabaseName}') IS NULL CREATE DATABASE [{TestDatabaseName}];";
    //     await cmd.ExecuteNonQueryAsync();
    // }

    private ServiceProvider InitializeServiceProvider()
    {
        var serviceCollection = new ServiceCollection();
        var configuration =
            new ConfigurationManager().AddInMemoryCollection(new List<KeyValuePair<string, string?>>
            {
                new("ConnectionStrings:DefaultConnection", GetConnectionString())
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
