﻿using System;
using FeatureFlags.Library.Core;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeatureFlags.Library.LaunchDarkly
{
    public static class DependencyInjection
    {
        public static IApplicationBuilder UseLaunchDarkly(this IApplicationBuilder builder,
            IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(_ =>
            {
                var client = builder.ApplicationServices.GetService<ILdClient>() as LdClient;
                client?.Dispose();
            }, null);
            return builder;
        }

        public static IServiceCollection AddLaunchDarkly(this IServiceCollection services,
            IConfiguration configuration,
            Action contextProviderSetup = null)
        {
            services.AddTransient<IFeatureService, FeatureService>();
            services.AddTransient<IJsonFeatureService, FeatureService>();
            services.AddTransient<Converter>();

            contextProviderSetup?.Invoke();

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