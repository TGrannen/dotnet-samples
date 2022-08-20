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
builder.Services.AddSingleton<ContainerService>();
builder.Services.AddSingleton<SeedService>();

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

var containerService = app.Services.GetRequiredService<ContainerService>();
var seeder = app.Services.GetRequiredService<SeedService>();

try
{
    await containerService.RunContainers();
    await seeder.CreateIndex(index);
    await seeder.SeedData(10);

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "ERROR");
}
finally
{
    await containerService.StopContainers();
}