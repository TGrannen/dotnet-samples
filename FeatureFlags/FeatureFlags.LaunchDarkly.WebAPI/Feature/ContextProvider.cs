using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class ContextProvider : IContextProvider
    {
        public Task<IFeatureContext> GetUserAsync()
        {
            return Task.FromResult(new FeatureContext { Key = "TEST" } as IFeatureContext);
        }
    }
}