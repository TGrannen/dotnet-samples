using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IJsonFeatureService
    {
        Task<T> GetJsonConfiguration<T>(string key, IFeatureContext context = null);
    }
}