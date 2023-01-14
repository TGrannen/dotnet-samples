using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith.Internal;

internal class FeatureService : IFeatureService, IJsonFeatureService
{
    private readonly IServiceProvider _provider;
    private readonly FlagsmithClient _client;
    private readonly ILogger<FeatureService> _logger;
    private static readonly Converter Converter = new();

    public FeatureService(IServiceProvider provider, FlagsmithClient client, ILogger<FeatureService> logger)
    {
        _provider = provider;
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IsEnabledAsync(string key)
    {
        var context = await GetProvider().GetUserAsync();
        return await IsEnabledAsync(key, context);
    }

    public async Task<bool> IsEnabledAsync(string key, bool defaultValue)
    {
        var context = await GetProvider().GetUserAsync();
        return await IsEnabledAsync(key, context, defaultValue);
    }

    public async Task<bool> IsEnabledAsync(string key, IFeatureContext context, bool defaultValue = false)
    {
        try
        {
            var config = Converter.Convert(context);
            var flags = await _client.GetIdentityFlags(config.Key, config.Traits);
            return await flags.IsFeatureEnabled(key);
        }
        catch (FlagsmithClientError e)
        {
            _logger.LogWarning(e, "Flagsmith encountered an error for {Flag}. Returning default value", key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during IsEnabledAsync check for Flag {Flag}. Returning default value", key);
        }

        return defaultValue;
    }

    public async Task<T> GetConfiguration<T>(string key) where T : class
    {
        var context = await GetProvider().GetUserAsync();
        return await GetConfiguration(key, context, default(T));
    }

    public async Task<T> GetConfiguration<T>(string key, T defaultValue) where T : class
    {
        var context = await GetProvider().GetUserAsync();
        return await GetConfiguration(key, context, defaultValue);
    }

    public async Task<T> GetConfiguration<T>(string key, IFeatureContext context, T defaultValue = default) where T : class
    {
        try
        {
            var config = Converter.Convert(context);
            var flags = await _client.GetIdentityFlags(config.Key, config.Traits);
            var value = await flags.GetFeatureValue(key);
            var result = JsonConvert.DeserializeObject<T>(value);
            return result ?? defaultValue;
        }
        catch (FlagsmithClientError e)
        {
            _logger.LogWarning(e, "Flagsmith encountered an error when getting Configuration for Flag {Flag}. Returning default value",
                key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during GetConfiguration check for Flag {Flag}. Returning default value", key);
        }

        return defaultValue;
    }

    private IContextProvider GetProvider()
    {
        if (_provider.GetService(typeof(IContextProvider)) is IContextProvider contextProvider)
        {
            return contextProvider;
        }

        throw new MissingContextProviderException();
    }
}