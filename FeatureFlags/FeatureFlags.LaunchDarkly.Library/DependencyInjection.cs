using System;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeatureFlags.LaunchDarkly.Library
{
    public static class DependencyInjection
    {
        public static IApplicationBuilder UseLaunchDarkly(this IApplicationBuilder builder,
            IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(_ =>
            {
                var provider = builder.ApplicationServices;
                var client = provider.GetService<ILdClient>() as LdClient;
                client?.Dispose();
            }, null);
            return builder;
        }

        public static IServiceCollection AddLaunchDarkly(this IServiceCollection services,
            IConfiguration configuration,
            Action userProviderSetup)
        {
            services.AddTransient<IFeatureService, FeatureService>();
            services.AddTransient<IJsonFeatureService, FeatureService>();

            userProviderSetup();

            services.Configure<LaunchDarklyConfig>(configuration.GetSection("Feature:LaunchDarkly"));
            services.AddSingleton<ILdClient>(provider =>
            {
                var config = provider.GetRequiredService<IOptions<LaunchDarklyConfig>>().Value;
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var ldConfig = Configuration.Builder(config.SdkKey)
                    .Logging(LdMicrosoftLogging.Adapter(loggerFactory))
                    .Build();
                return new LdClient(ldConfig);
            });

            return services;
        }

        private class LaunchDarklyConfig
        {
            public string SdkKey { get; set; }
        }
    }
}