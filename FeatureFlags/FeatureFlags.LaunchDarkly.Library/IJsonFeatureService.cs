using System.Threading.Tasks;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IJsonFeatureService
    {
        Task<T> GetJsonConfiguration<T>(string key, IFeatureContext context = null);
    }
}