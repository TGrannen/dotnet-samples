using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IContextProvider
    {
        Task<IFeatureContext> GetUserAsync();
    }
}