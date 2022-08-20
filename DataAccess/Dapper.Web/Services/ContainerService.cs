using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Dapper.Web.Services;

/// <summary>
/// Run a Postgres container with randomized port number so that there are no issues running a demo with existing postgres instances
/// </summary>
public interface IContainerService
{
    public PostgreSqlTestcontainer? Container { get; }
    Task RunContainer();
    Task StopContainer();
}

public class ContainerService : IContainerService
{
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(ILogger<ContainerService> logger)
    {
        _logger = logger;
    }

    public PostgreSqlTestcontainer? Container { get; private set; }

    public async Task RunContainer()
    {
        var containersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "dapperdb",
                Username = "test",
                Password = "test-password",
            })
            .WithName("postgres-dotnet-samples");

        _logger.LogInformation("Building Container");
        Container = containersBuilder.Build();
        _logger.LogInformation("Starting Container");
        await Container.StartAsync();
    }

    public async Task StopContainer()
    {
        if (Container != null)
        {
            await Container.DisposeAsync();
        }
    }
}