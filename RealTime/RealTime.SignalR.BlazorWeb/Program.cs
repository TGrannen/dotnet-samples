using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using RealTime.SignalR.BlazorWeb;
using RealTime.SignalR.BlazorWeb.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.Configure<HubConfiguration>(builder.Configuration.GetSection("HubHost"));
builder.Services.AddTransient<IHubConfiguration>(provider => provider.GetRequiredService<IOptions<HubConfiguration>>().Value);

await builder.Build().RunAsync();