using Configuration.Web.Extensions;
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
            
            services.Configure<ISettings2, Settings2>(Configuration.GetSection("Settings2"));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}