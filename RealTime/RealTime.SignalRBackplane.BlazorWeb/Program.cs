using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using RealTime.SignalRBackplane.BlazorWeb;
using RealTime.SignalRBackplane.BlazorWeb.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.Configure<HubConfiguration>(builder.Configuration.GetSection("Hub"));
builder.Services.AddTransient<IHubConfiguration>(provider => provider.GetRequiredService<IOptions<HubConfiguration>>().Value);

await builder.Build().RunAsync();