using System.Diagnostics;
using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Nest;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var index = "test";
var client = GetConnection("http://localhost:9200", index);
builder.Services.AddSingleton(client);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var networkBuilder = new TestcontainersNetworkBuilder()
    .WithName("elastic")
    .WithDriver(NetworkDriver.Bridge);

var network = networkBuilder.Build();
try
{
    Log.Information("Creating Network");
    await network.CreateAsync();

    var name = "elasticsearch-dotnet-samples";
    var containersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("elasticsearch:8.3.3")
        .WithName(name)
        .WithEnvironment("xpack.security.enabled", "false")
        .WithEnvironment("discovery.type", "single-node")
        .WithPortBinding(9200)
        .WithPortBinding(9300)
        .WithNetwork(network)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9300));

    var containersBuilder2 = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("kibana:8.3.3")
        .WithName("kibana-dotnet-samples")
        // .WithEnvironment("ELASTICSEARCH_HOSTS", $"http://{name}:9200")
        .WithPortBinding(5601)
        .WithNetwork(network);

    Log.Information("Building Containers");
    await using var containers = containersBuilder.Build();
    // await using var containers2 = containersBuilder2.Build();
    Log.Information("Starting Containers");
    await containers.StartAsync();
    // await containers2.StartAsync();

    var logs = GetLogs();
    Log.Information("Creating Index");
    await CreateIndex(client, logs, index);

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "ERROR");
}
finally
{
    await network.DeleteAsync();
}

List<LogDocument> GetLogs()
{
    Randomizer.Seed = new Random(123456);
    var logsFaker = new Faker<LogDocument>()
        .RuleFor(o => o.Id, Guid.NewGuid)
        .RuleFor(o => o.Body, f => f.Random.Words());
    return logsFaker.Generate(100);
}

ElasticClient GetConnection(string connectionUri, string indexname)
{
    Uri node = new Uri(connectionUri);
    ConnectionSettings settings = new ConnectionSettings(node)
        .DefaultMappingFor<LogDocument>(m => m
            .IndexName(indexname)
        );
    settings.EnableApiVersioningHeader();
    
    return new ElasticClient(settings);
}

async Task CreateIndex(ElasticClient client, List<LogDocument> documents, string indexName)
{
    var timer = new Stopwatch();
    timer.Start();

    var createIndexResponse = client.Indices.Create(indexName, c => c.Map<LogDocument>(m => m.AutoMap<LogDocument>()));
    foreach (var document in documents)
    {
        await client.CreateDocumentAsync(new LogDocument()
        {
            Body = document.Body.Trim()
        });
    }

    timer.Stop();
    Log.Information("Index created in {Elapsed}ms with {Count} element", timer.Elapsed, documents.Count);
}

public class LogDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Body { get; set; }
}