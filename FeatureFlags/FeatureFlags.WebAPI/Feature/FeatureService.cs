using Microsoft.FeatureManagement;

namespace FeatureFlags.WebAPI.Feature;

public class FeatureService : IFeatureService
{
    private readonly IFeatureManager _manager;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IFeatureManager manager, ILogger<FeatureService> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    public async Task<bool> IsEnabledAsync(Features feature)
    {
        var featureString = GetFeatureString(feature);

        if (string.IsNullOrEmpty(featureString))
        {
            _logger.LogWarning("Couldn't find a valid setting for feature: {Feature} Defaulting to feature being disabled", feature);
            return false;
        }

        return await _manager.IsEnabledAsync(featureString);
    }


    public async Task<bool> IsNotEnabledAsync(Features feature)
    {
        return !await IsEnabledAsync(feature);
    }

    public async Task<bool> IsEnabledAsync<TContext>(Features feature, TContext context)
    {
        var featureString = GetFeatureString(feature);

        if (string.IsNullOrEmpty(featureString))
        {
            _logger.LogWarning("Couldn't find a valid setting for feature: {Feature} Defaulting to feature being disabled", feature);
            return false;
        }

        return await _manager.IsEnabledAsync(featureString, context);
    }

    public async Task<bool> IsNotEnabledAsync<TContext>(Features feature, TContext context)
    {
        return !await IsEnabledAsync(feature, context);
    }

    private static string GetFeatureString(Features feature)
    {
        var featureString = feature switch
        {
            Features.ShouldHaveOnlyOne => "ShouldHaveOnlyOne",
            Features.AllowedForEndpoint => "AllowedForEndpoint",
            Features.AllowForMinNumber => "AllowForMinNumber",
            _ => string.Empty
        };
        return featureString;
    }
}