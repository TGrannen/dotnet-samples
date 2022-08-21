using Dapper.CleanArchitecture.Application;
using Dapper.CleanArchitecture.Infrastructure;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;
using Dapper.CleanArchitecture.Web.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddSingleton<IContainerService, ContainerService>();
builder.Services.AddTransient<IDbConnectionStringProvider, DbConnectionStringProvider>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

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

try
{
    await containerService.RunContainer();
    var seeder = app.Services.CreateScope().ServiceProvider.GetService<ISeedService>();
    if (seeder != null)
    {
        await seeder.CreateDatabase();
    }

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

public abstract partial class Program
{
}