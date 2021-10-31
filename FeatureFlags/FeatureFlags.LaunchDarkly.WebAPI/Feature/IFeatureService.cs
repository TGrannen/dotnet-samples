using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(string key, IFeatureContext context = null);
    }
}