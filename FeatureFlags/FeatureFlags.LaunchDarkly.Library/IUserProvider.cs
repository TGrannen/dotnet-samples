using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IUserProvider
    {
        User GetUser();
    }
}