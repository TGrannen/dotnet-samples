using FeatureFlags.Library.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;

namespace FeatureFlags.Library.SplitIO;

public static class DependencyInjection
{
    public static IApplicationBuilder UseSplit(this IApplicationBuilder builder, IHostApplicationLifetime appLifetime)
    {
        appLifetime.ApplicationStopped.Register(_ =>
        {
            var logger = builder.ApplicationServices.GetRequiredService<ILogger<FeatureService>>();
            var client = builder.ApplicationServices.GetRequiredService<ISplitClient>();
            logger.LogInformation("Shutting down Split SDK");
            client.Destroy();
            logger.LogInformation("Shut down Split SDK");
        }, null);
        builder.ApplicationServices.GetService<ILoggerFactory>().AddSplitLogs();
        return builder;
    }

    public static IServiceCollection AddSplit(this IServiceCollection services, Action<SplitConfig>? configAction = null)
    {
        services.AddSingleton<ISplitFactory, SplitFactory>(s =>
        {
            var config = new SplitConfig();
            configAction?.Invoke(config);
            var key = configAction != null ? config.SdkKey : s.GetRequiredService<IConfiguration>().GetValue<string>("Split:ApiKey");
            return new SplitFactory(key);
        });

        services.AddSingleton<ISplitClient>(s =>
        {
            var logger = s.GetRequiredService<ILogger<FeatureService>>();
            ISplitClient client;
            try
            {
                client = s.GetRequiredService<ISplitFactory>().Client();
                client.BlockUntilReady(10000);
                logger.LogInformation("Split SDK ready");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start Split IO Client");
                throw;
            }

            return client;
        });

        services.AddTransient<IFeatureService, FeatureService>();
        services.AddTransient<IJsonFeatureService, FeatureService>();

        return services;
    }
}