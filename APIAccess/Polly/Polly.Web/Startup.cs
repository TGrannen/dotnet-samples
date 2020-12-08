using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Web.Services;
using System;
using System.Net.Http;

namespace Polly.Web
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Polly.Web", Version = "v1" });
            });

            services.AddHttpClient("GitHub", client =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });

            var registry = services.AddPolicyRegistry();

            registry.Add("retry", GetRetryPolicy());
            registry.Add("circuit", GetCircuitBreakerPolicy());

            services.AddSingleton<IGitHubService, GitHubService>();

            services
                .AddHttpClient<IFlakyGitHubService, UnReliableGitHubService>((provider, client) =>
                {
                    var uriString = Configuration.GetValue<string>("FlakyServerUri");
                    client.BaseAddress = new Uri(uriString);
                })
                .AddPollyContextLoggingNoOpPolicy<UnReliableGitHubService>()
                .AddPolicyHandlerFromRegistry("circuit")
                .AddPolicyHandlerFromRegistry("retry")
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polly.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                    onRetry: (exception, duration, retryCount, context) =>
                    {
                        context.GetLogger()
                            .LogWarning("Retry Number: {RetryCount}  Waiting: {Duration:#}ms, due to: {Message}.",
                                retryCount,
                                duration.TotalMilliseconds,
                                exception.Exception?.Message ?? exception.Result.ToString());
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
                    onBreak: (result, state, duration, context) =>
                    {
                        context.GetLogger().LogWarning("CircuitBreaker PreviousState:{PreviousState} State:{State} Duration {Duration:#}", state, CircuitState.Open, duration.TotalSeconds);
                    },
                    onReset: context =>
                    {
                        context.GetLogger().LogWarning("CircuitBreaker State:{State}", CircuitState.Closed);
                    },
                    () => { });
        }
    }
}