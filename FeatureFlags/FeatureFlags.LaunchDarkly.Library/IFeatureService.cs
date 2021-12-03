using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(string key, IFeatureContext context = null, bool defaultValue = false);
    }
}