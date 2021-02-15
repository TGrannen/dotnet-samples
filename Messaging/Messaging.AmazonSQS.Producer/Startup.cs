using MassTransit;
using Messaging.Configuration;
using Messaging.Configuration.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Messaging.AmazonSQS.Producer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messaging.AmazonSQS.Producer", Version = "v1" });
            });

            services.AddMassTransit(x =>
            {
                x.UsingAmazonSqs((context, cfg) =>
                {
                    var config = context.GetRequiredService<IAwsSqsConfig>();
                    cfg.Host(config.HostName, h =>
                    {
                        h.AccessKey(config.AccessKey);
                        h.SecretKey(config.SecretKey);

                        //// following are OPTIONAL

                        //// specify a scope for all queues
                        //h.Scope("dev");

                        //// scope topics as well
                        //h.EnableScopedTopics();
                    });
                });
            });

            services.AddAwsSqsConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging.AmazonSQS.Producer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}