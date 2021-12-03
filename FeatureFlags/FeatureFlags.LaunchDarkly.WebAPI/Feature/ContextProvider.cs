using FeatureFlags.LaunchDarkly.Library;
using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class ContextProvider : IContextProvider
    {
        public IFeatureContext GetUser()
        {
            return new FeatureContext { Key = "TEST" };
        }
    }
}