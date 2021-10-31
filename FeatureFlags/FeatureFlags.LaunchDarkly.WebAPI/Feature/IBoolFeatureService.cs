using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    /// <summary>
    /// Abstraction over IFeatureManager
    /// </summary>
    public interface IBoolFeatureService
    {
        Task<bool> IsEnabledAsync(Features feature);
        Task<bool> IsEnabledAsync(Features feature, IFeatureContext context);
    }
}