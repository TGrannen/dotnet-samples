using System.Threading.Tasks;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(string key, IFeatureContext context = null);
    }
}