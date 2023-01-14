using FeatureFlags.Library.Flagsmith.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith;

public static class DependencyInjection
{
    public static IServiceCollection AddFlagsmith(this IServiceCollection services, Action<FlagsmithConfig>? configAction = null)
    {
        services.AddSingleton<FlagsmithConfig>(s =>
        {
            var config = new FlagsmithConfig();
            configAction?.Invoke(config);
            var key = configAction != null ? config.SdkKey : s.GetRequiredService<IConfiguration>().GetValue<string>("Flagsmith:ApiKey");
            return new FlagsmithConfig
            {
                SdkKey = key
            };
        });

        services.AddSingleton<FlagsmithClient>(s =>
        {
            var logger = s.GetRequiredService<ILogger<FeatureService>>();
            FlagsmithClient client;
            try
            {
                var config = s.GetRequiredService<FlagsmithConfig>();
                client = new FlagsmithClient(config.SdkKey);
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