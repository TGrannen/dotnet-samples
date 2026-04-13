using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ApiService.Api.Infrastructure.Caching;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        var fusionBuilder = builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(new FusionCacheEntryOptions { Duration = TimeSpan.FromMinutes(2), })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer());

        var useRedisCache = !builder.Environment.IsEnvironment("Testing")
                            && !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("cache"));

        if (!useRedisCache)
        {
            return builder;
        }

        builder.AddRedisDistributedCache("cache");
        fusionBuilder.WithRegisteredDistributedCache();
        return builder;
    }
}
