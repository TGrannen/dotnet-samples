using System;
using FeatureFlags.Library.Core;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.LaunchDarkly
{
    public static class DependencyInjection
    {
        public static IApplicationBuilder UseLaunchDarkly(this IApplicationBuilder builder,
            IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(_ =>
            {
                // To ensure that the client flushes all remaining tracking events before shutting down
                // https://docs.launchdarkly.com/sdk/features/shutdown
                var client = builder.ApplicationServices.GetService<ILdClient>() as LdClient;
                client?.Dispose();
            }, null);
            return builder;
        }

        public static IServiceCollection AddLaunchDarkly(this IServiceCollection services,
            Action<LaunchDarklyConfig> configAction)
        {
            services.AddTransient<IFeatureService, FeatureService>();
            services.AddTransient<IJsonFeatureService, FeatureService>();
            services.AddTransient<Converter>();

            services.AddSingleton<ILdClient>(provider =>
            {
                var config = new LaunchDarklyConfig();
                configAction.Invoke(config);
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                var ldConfig = Configuration.Builder(config.SdkKey)
                    .Logging(LdMicrosoftLogging.Adapter(loggerFactory))
                    .Build();
                return new LdClient(ldConfig);
            });

            return services;
        }
    }
}