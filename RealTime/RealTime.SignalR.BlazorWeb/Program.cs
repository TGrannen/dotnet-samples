using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.SignalR.BlazorWeb.Configuration;

namespace RealTime.SignalR.BlazorWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            });

            builder.Services.Configure<HubConfiguration>(builder.Configuration.GetSection("HubHost"));
            builder.Services.AddTransient<IHubConfiguration>(provider => provider.GetRequiredService<IOptions<HubConfiguration>>().Value);
            
            await builder.Build().RunAsync();
        }
    }
}