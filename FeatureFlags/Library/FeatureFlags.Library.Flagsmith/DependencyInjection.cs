using FeatureFlags.Library.Flagsmith.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith;

public static class DependencyInjection
{
    public static IServiceCollection AddFlagsmith(this IServiceCollection services, Action<FlagsmithConfig>? configAction = null)
    {
        services.AddSingleton<FlagsmithConfig>(provider =>
        {
            var config = new FlagsmithConfig();
            configAction?.Invoke(config);
            var key = !string.IsNullOrEmpty(config.SdkKey)
                ? config.SdkKey
                : provider.GetRequiredService<IConfiguration>().GetValue<string>("Flagsmith:ApiKey");
            var analytics = config.EnableAnalytics
                ? config.EnableAnalytics
                : provider.GetRequiredService<IConfiguration>().GetValue("Flagsmith:EnableAnalytics", false);
            return new FlagsmithConfig
            {
                SdkKey = key,
                EnableAnalytics = analytics
            };
        });

        services.AddSingleton<FlagsmithClient>(s =>
        {
            var logger = s.GetRequiredService<ILogger<FeatureService>>();
            var clientLogger = s.GetRequiredService<ILogger<FlagsmithClient>>();
            FlagsmithClient client;
            try
            {
                var config = s.GetRequiredService<FlagsmithConfig>();
                client = new FlagsmithClient(config.SdkKey, logger: clientLogger, enableAnalytics: config.EnableAnalytics);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start Flagsmith client");
                throw;
            }

            return client;
        });

        services.AddTransient<IFeatureService, FeatureService>();
        services.AddTransient<IJsonFeatureService, FeatureService>();
        services.AddTransient<IEnvFeatureService, EnvFeatureService>();
        services.AddTransient<IEnvJsonFeatureService, EnvFeatureService>();
        services.AddSingleton<IEnvFlagsProvider, EnvFlagsProvider>();

        return services;
    }
}