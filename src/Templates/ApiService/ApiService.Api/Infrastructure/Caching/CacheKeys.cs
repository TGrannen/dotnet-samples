namespace ApiService.Api.Infrastructure.Caching;

internal static class CacheKeys
{
    public static string ForProductId(Guid id) => $"product:{id}";
}
