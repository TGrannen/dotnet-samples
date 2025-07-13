using Scalar.AspNetCore;
using Serilog;
using TUnitTesting.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

if (!builder.Environment.IsEnvironment("Testing"))
{
    app.UseSerilogRequestLogging();
}
else
{
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}