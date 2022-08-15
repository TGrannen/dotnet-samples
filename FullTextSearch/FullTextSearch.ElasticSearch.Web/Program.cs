using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FullTextSearch.ElasticSearch.Web.Services;
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

var index = "my-index";
var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultMappingFor<LogDocument>(m => m.IndexName(index));
settings.EnableApiVersioningHeader();
var client = new ElasticClient(settings);

builder.Services.AddSingleton(client);
builder.Services.AddSingleton<Seed>();

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
        .WithEnvironment("ELASTICSEARCH_HOSTS", $"http://{name}:9200")
        .WithPortBinding(5601)
        .WithNetwork(network);

    Log.Information("Building Containers");
    await using var containers = containersBuilder.Build();
    await using var containers2 = containersBuilder2.Build();
    Log.Information("Starting Containers");
    await containers.StartAsync();
    await containers2.StartAsync();

    var seeder = app.Services.GetRequiredService<Seed>();
    await seeder.SeedData(index);

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