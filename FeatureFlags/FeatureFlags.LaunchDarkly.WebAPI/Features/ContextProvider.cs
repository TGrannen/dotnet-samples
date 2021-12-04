using System.Threading.Tasks;
using FeatureFlags.Library.Core;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Features
{
    class ContextProvider : IContextProvider
    {
        public Task<IFeatureContext> GetUserAsync()
        {
            return Task.FromResult(new FeatureContext { Key = "TEST" } as IFeatureContext);
        }
    }
}