using System.Threading.Tasks;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.Library.Core
{
    public interface IJsonFeatureService
    {
        Task<T> GetConfiguration<T>(string key) where T : class;
        Task<T> GetConfiguration<T>(string key, T defaultValue) where T : class;
        Task<T> GetConfiguration<T>(string key, IFeatureContext context, T defaultValue = default) where T : class;
    }
}