using Configuration.Web.Extensions;
using Configuration.Web.Models;
using Configuration.Web.Providers;
using Configuration.Web.Providers.CustomProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Configuration.Web", Version = "v1" });
            });


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
            services.Configure<Settings3>(Configuration.GetSection("Settings3"));
            services.AddScoped<ISettings3>(provider => provider.GetService<IOptionsSnapshot<Settings3>>()?.Value);
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Configuration.Web v1"));

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}