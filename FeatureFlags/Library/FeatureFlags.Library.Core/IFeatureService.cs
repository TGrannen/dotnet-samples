using System.Threading.Tasks;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.Library.Core
{
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(string key);
        Task<bool> IsEnabledAsync(string key, bool defaultValue);
        Task<bool> IsEnabledAsync(string key, IFeatureContext context, bool defaultValue = false);
    }
}