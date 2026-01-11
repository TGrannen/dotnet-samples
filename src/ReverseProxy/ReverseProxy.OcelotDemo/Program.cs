using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.Services.AddControllers();

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSerilogRequestLogging();

#pragma warning disable ASP0014
// Disabled warning because this is required to make this work
// instead of the normal app.MapControllers() call
app.UseEndpoints(endpoints => endpoints.MapControllers());
#pragma warning restore ASP0014

await app.UseOcelot();
app.Run();