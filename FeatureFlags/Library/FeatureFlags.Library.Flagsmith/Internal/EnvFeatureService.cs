using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith.Internal;

internal class EnvFeatureService : IEnvFeatureService, IEnvJsonFeatureService
{
    private readonly IEnvFlagsProvider _flagsProvider;
    private readonly ILogger<EnvFeatureService> _logger;

    public EnvFeatureService(IEnvFlagsProvider flagsProvider, ILogger<EnvFeatureService> logger)
    {
        _flagsProvider = flagsProvider;
        _logger = logger;
    }

    public async Task<bool> IsEnabledAsync(string key)
    {
        return await IsEnabledAsync(key, false);
    }

    public async Task<bool> IsEnabledAsync(string key, bool defaultValue)
    {
        try
        {
            var flags = await _flagsProvider.GetFlags();
            return await flags.IsFeatureEnabled(key);
        }
        catch (FlagsmithClientError e)
        {
            _logger.LogWarning(e, "Flagsmith encountered an error when getting Env Flag {Flag}. Returning default value",
                key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during env Flag check for Flag {Flag}. Returning default value", key);
        }

        return defaultValue;
    }

    public async Task<T> GetConfiguration<T>(string key) where T : class
    {
        return await GetConfiguration(key, default(T));
    }

    public async Task<T> GetConfiguration<T>(string key, T defaultValue) where T : class
    {
        try
        {
            var flags = await _flagsProvider.GetFlags();
            var value = await flags.GetFeatureValue(key);
            var result = JsonConvert.DeserializeObject<T>(value);
            return result ?? defaultValue;
        }
        catch (FlagsmithClientError e)
        {
            _logger.LogWarning(e, "Flagsmith encountered an error when getting Env Configuration for Flag {Flag}. Returning default value",
                key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Env GetConfiguration check for Flag {Flag}. Returning default value", key);
        }

        return defaultValue;
    }
}