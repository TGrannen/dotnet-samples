using Dapper.Web.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddSingleton<SeedService>();
builder.Services.AddSingleton<IContainerService, ContainerService>();
builder.Services.AddTransient<IConnectionStringProvider, ConnectionStringProvider>();
builder.Services.AddTransient<IConnectionProvider, ConnectionProvider>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var containerService = app.Services.GetRequiredService<IContainerService>();
var seeder = app.Services.GetRequiredService<SeedService>();

try
{
    await containerService.RunContainer();
    await seeder.CreateDatabase();

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "ERROR");
}
finally
{
    await containerService.StopContainer();
}