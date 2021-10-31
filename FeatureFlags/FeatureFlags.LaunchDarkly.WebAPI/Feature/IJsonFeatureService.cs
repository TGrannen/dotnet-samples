using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    public interface IJsonFeatureService
    {
        Task<T> GetJsonConfiguration<T>(string key, IFeatureContext context = null);
    }
}