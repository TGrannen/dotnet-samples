using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IJsonFeatureService
    {
        Task<T> GetConfiguration<T>(string key, IFeatureContext context = null, T defaultValue = default) where T : class;
    }
}