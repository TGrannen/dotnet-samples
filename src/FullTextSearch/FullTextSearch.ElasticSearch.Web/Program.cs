using FullTextSearch.ElasticSearch.Web.Services;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration), writeToProviders: true);

// Add services to the container.

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var index = builder.Configuration.GetValue("Elastic:IndexName", "my-index");
var connectionString = builder.Configuration.GetConnectionString("elasticsearch")!;

builder.Services.AddSingleton<ElasticClient>(_ =>
{
    var connectionSettings = new ConnectionSettings(new Uri(connectionString))
        .DefaultMappingFor<LogDocument>(m => m.IndexName(index));
    connectionSettings.EnableApiVersioningHeader();
    return new ElasticClient(connectionSettings);
});

builder.Services.AddSingleton<SeedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var seeder = app.Services.GetRequiredService<SeedService>();

try
{
    await seeder.CreateIndex(index);
    await seeder.SeedData(10);

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "ERROR");
}