using FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLaunchDarkly(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBoolFeatureService, LaunchDarklyFeatureService>();
            services.AddTransient<IFeatureKeyConverter, FeatureKeyConverter>();
            services.AddTransient<IUserConverter, UserConverter>();

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