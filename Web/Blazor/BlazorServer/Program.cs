using Microsoft.Extensions.Hosting;
using BlazorServer.Data;
using BlazorServer.Services;
using Fluxor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var services = builder.Services;
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddSingleton<WeatherForecastService>();

builder.Services.AddMudServices();

// Add Fluxor
services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools();
});

services.AddScoped<WeatherService>();

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

await app.RunAsync();