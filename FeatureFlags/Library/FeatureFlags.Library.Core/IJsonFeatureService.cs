using System.Threading.Tasks;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.Library.Core
{
    public interface IJsonFeatureService
    {
        Task<T> GetConfiguration<T>(string key, IFeatureContext context = null, T defaultValue = default) where T : class;
    }
}