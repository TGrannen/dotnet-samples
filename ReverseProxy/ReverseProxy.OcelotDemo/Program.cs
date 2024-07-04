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

// Must use UseEndpoints here or it will not work 🤷‍♂️
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.UseOcelot().Wait();
app.Run();