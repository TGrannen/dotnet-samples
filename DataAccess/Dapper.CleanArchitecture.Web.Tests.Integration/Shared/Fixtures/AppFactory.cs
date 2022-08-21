using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;
using Dapper.CleanArchitecture.Web.Services;
using Dapper.CleanArchitecture.Web.Tests.Integration.Shared.Providers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared.Fixtures;

public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "dapperdb",
            Username = "postgres",
            Password = "test-password",
        }).Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IDbConnectionStringProvider>(_ => new DbConnectionStringProvider
            {
                ConnectionString = _container.ConnectionString
            });
            services.RemoveAll(typeof(IContainerService));
            services.AddSingleton<IContainerService>(_ => new DummyContainerService
            {
                Container = _container
            });
        }).UseEnvironment("Testing");
    }


    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _container?.StopAsync()!;
    }
}