using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace FullTextSearch.ElasticSearch.Web.Services;

public class ContainerService
{
    private readonly ILogger<ContainerService> _logger;
    private IDockerNetwork? _network;
    private TestcontainersContainer? _containers;
    private TestcontainersContainer? _containers2;

    public ContainerService(ILogger<ContainerService> logger)
    {
        _logger = logger;
    }

    public async Task RunContainers()
    {
        var networkBuilder = new TestcontainersNetworkBuilder()
            .WithName("elastic")
            .WithDriver(NetworkDriver.Bridge);

        _network = networkBuilder.Build();
        _logger.LogInformation("Creating Network");
        await _network.CreateAsync();

        var name = "elasticsearch-dotnet-samples";
        var containersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("elasticsearch:8.3.3")
            .WithName(name)
            .WithEnvironment("xpack.security.enabled", "false")
            .WithEnvironment("discovery.type", "single-node")
            .WithPortBinding(9200)
            .WithPortBinding(9300)
            .WithNetwork(_network)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9200));

        var containersBuilder2 = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("kibana:8.3.3")
            .WithName("kibana-dotnet-samples")
            .WithEnvironment("ELASTICSEARCH_HOSTS", $"http://{name}:9200")
            .WithPortBinding(5601)
            .WithNetwork(_network);

        _logger.LogInformation("Building Containers");
        _containers = containersBuilder.Build();
        _containers2 = containersBuilder2.Build();
        _logger.LogInformation("Starting Containers");
        await _containers.StartAsync();
        await _containers2.StartAsync();
    }

    public async Task StopContainers()
    {
        await DisposeContainer(_containers);
        await DisposeContainer(_containers2);

        await DeleteNetwork();
    }

    private static async Task DisposeContainer(IAsyncDisposable? container)
    {
        if (container != null)
        {
            await container.DisposeAsync();
        }
    }

    private async Task DeleteNetwork()
    {
        if (_network != null)
        {
            await _network.DeleteAsync();
        }
    }
}