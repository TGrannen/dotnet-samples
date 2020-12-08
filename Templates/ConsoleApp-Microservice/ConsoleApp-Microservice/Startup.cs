using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp_Microservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthEndpoints();

            services.AddHostedService<ExampleBackgroundService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthEndpoint();
        }
    }
}