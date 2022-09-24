namespace FeatureFlags.WebAPI.Feature;

/// <summary>
/// Abstraction over IFeatureManager
/// </summary>
public interface IFeatureService
{
    Task<bool> IsEnabledAsync(Features feature);
    Task<bool> IsNotEnabledAsync(Features feature);
    Task<bool> IsEnabledAsync<TContext>(Features feature, TContext context);
    Task<bool> IsNotEnabledAsync<TContext>(Features feature, TContext context);
}