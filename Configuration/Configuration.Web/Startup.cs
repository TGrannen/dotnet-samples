using Configuration.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Configuration.Web
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
            services.AddControllers();

            services.Configure<Settings1>(Configuration.GetSection("Settings1"));

            services.AddScoped<ISettings1>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                bool shouldUseInitialValue = config.GetValue<bool>("InjectedInterfaceShouldUsingInitialValue");
                if (shouldUseInitialValue)
                {
                    return provider.GetService<IOptions<Settings1>>()?.Value;
                }

                return provider.GetService<IOptionsSnapshot<Settings1>>()?.Value;
            });
        }

#pragma warning disable IDE0060 // Remove unused parameter

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}