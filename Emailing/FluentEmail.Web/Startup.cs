using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace FluentEmail.Web
{
    public class Startup
    {
        private string _contentRoot;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _contentRoot = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FluentEmail.Web", Version = "v1" });
            });

            ConfigureFluentEmail(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FluentEmail.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureFluentEmail(IServiceCollection services, IConfiguration configuration)
        {
            var fromAddress = configuration.GetValue("EmailConfig:FromAddress", "default-from@test.com");
            //var apiKey = configuration.GetValue("EmailConfig:SendGrid:APIKey", "dummy-api-key");
            //var sandBoxMode = configuration.GetValue("EmailConfig:SendGrid:UseSandbox", true);

            var server = configuration.GetValue<string>("EmailConfig:SMTPConfig:Server");
            var port = configuration.GetValue<int>("EmailConfig:SMTPConfig:Port");

            services
                .AddFluentEmail(fromAddress)
                .AddRazorRenderer(Path.Combine(_contentRoot, "Templates/"))
                //.AddSendGridSender(apiKey, sandBoxMode);
                .AddSmtpSender(server, port);
        }
    }
}