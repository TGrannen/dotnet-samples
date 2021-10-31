using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    public interface IUserProvider
    {
        User GetUser();
    }
}