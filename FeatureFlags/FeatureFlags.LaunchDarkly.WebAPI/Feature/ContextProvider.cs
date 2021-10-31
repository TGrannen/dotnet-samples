using FeatureFlags.LaunchDarkly.Library;
using FeatureFlags.LaunchDarkly.Library.Context;
using LaunchDarkly.Sdk;

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