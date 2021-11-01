using FeatureFlags.LaunchDarkly.Library.Context;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IContextProvider
    {
        IFeatureContext GetUser();
    }
}