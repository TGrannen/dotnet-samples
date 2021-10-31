using System.Threading.Tasks;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    /// <summary>
    /// Abstraction over IFeatureManager
    /// </summary>
    public interface IBoolFeatureService
    {
        Task<bool> IsEnabledAsync(Features feature);
        Task<bool> IsEnabledAsync<TContext>(Features feature, TContext context);
    }
}