using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    public interface IJsonFeatureService
    {
        Task<T> GetFeatureConfigurationAsync<T>(Features feature);
        Task<T> GetFeatureConfigurationAsync<T>(Features feature, IFeatureContext context);
    }
}