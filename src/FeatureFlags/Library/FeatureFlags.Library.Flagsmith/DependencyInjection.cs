using FeatureFlags.Library.Flagsmith.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith;

public static class DependencyInjection
{
    public static IServiceCollection AddFlagsmith(this IServiceCollection services, Action<FlagsmithConfiguration>? configAction = null)
    {
        services.AddSingleton<FlagsmithConfiguration>(provider =>
        {
            var config = new FlagsmithConfiguration();
            configAction?.Invoke(config);
            var configuration = provider.GetRequiredService<IConfiguration>();
            var analytics = config.EnableAnalytics
                ? config.EnableAnalytics
                : configuration.GetValue("Flagsmith:EnableAnalytics", false);
            return new FlagsmithConfiguration
            {
                
                EnableAnalytics = analytics
            };
        });

        services.AddSingleton<FlagsmithClient>(s =>
        {
            var logger = s.GetRequiredService<ILogger<FeatureService>>();
            FlagsmithClient client;
            try
            {
                var config = s.GetRequiredService<FlagsmithConfiguration>();
                client = new FlagsmithClient(config);
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