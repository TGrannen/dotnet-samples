using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQExample.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddNpgsqlDbContext<TestDbContext>(connectionName: "Database");

builder.Services.AddTickerQ(opt =>
{
    // Set your class that implements ITickerExceptionHandler.  
    // opt.SetExceptionHandler<MyExceptionHandlerClass>();
    // Set the max thread concurrency for Ticker (default: Environment.ProcessorCount).
    opt.SetMaxConcurrency(4);
    opt.SetInstanceIdentifier(builder.Configuration.GetValue<string>("InstanceIdentifier"));

    
    opt.AddDashboard(basePath: "/tickerq-dashboard"); 
    opt.AddDashboardBasicAuth(); 
    
    opt.AddOperationalStore<TestDbContext>(efOpt =>
    { 
        efOpt.UseModelCustomizerForMigrations(); // Applies custom model customization only during EF Core migrations
        efOpt.CancelMissedTickersOnApplicationRestart(); // Useful in distributed mode
    }); 
});

builder.Services.AddControllers();

builder.AddServiceDefaults();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbcontext = scope.ServiceProvider.GetService<TestDbContext>();
    await dbcontext.Database.MigrateAsync();

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseTickerQ();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();