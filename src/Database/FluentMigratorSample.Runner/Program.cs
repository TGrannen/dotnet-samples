using FluentMigratorSample.Runner.Migrations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        // Define the assembly containing the migrations, maintenance migrations and other customizations
        .ScanIn(typeof(AddLogTable).Assembly).For.All());

var app = builder.Build();

using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

if (runner.HasMigrationsToApplyUp())
{
    logger.LogInformation("Starting Migration");
    runner.HasMigrationsToApplyUp();
    runner.MigrateUp();
    logger.LogInformation("Migration Complete");
}
else
{
    logger.LogInformation("No migrations to apply");
}