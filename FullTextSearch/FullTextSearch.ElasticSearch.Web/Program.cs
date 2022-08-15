using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var networkBuilder = new TestcontainersNetworkBuilder()
    .WithName("elastic")
    .WithDriver(NetworkDriver.Bridge);

var network = networkBuilder.Build();
try
{
    await network.CreateAsync();
    var name = "elasticsearch-dotnet-samples";
    var containersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("elasticsearch:8.3.3")
        .WithName(name)
        .WithEnvironment("xpack.security.enabled", "false")
        .WithEnvironment("discovery.type", "single-node")
        .WithPortBinding(9200)
        .WithPortBinding(9300)
        .WithNetwork(network);

    var containersBuilder2 = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("kibana:8.3.3")
        .WithName("kibana-dotnet-samples")
        // .WithEnvironment("ELASTICSEARCH_HOSTS", $"http://{name}:9200")
        .WithPortBinding(5601)
        .WithNetwork(network);

    await using var containers = containersBuilder.Build();
    await using var containers2 = containersBuilder2.Build();
    await containers.StartAsync();
    await containers2.StartAsync();

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