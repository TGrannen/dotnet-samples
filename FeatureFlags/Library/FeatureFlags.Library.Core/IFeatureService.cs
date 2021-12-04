using System.Threading.Tasks;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.Library.Core
{
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(string key, IFeatureContext context = null, bool defaultValue = false);
    }
}