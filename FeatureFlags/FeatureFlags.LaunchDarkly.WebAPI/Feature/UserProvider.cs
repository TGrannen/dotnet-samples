using FeatureFlags.LaunchDarkly.Library;
using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class UserProvider : IUserProvider
    {
        public User GetUser()
        {
            return User.WithKey("TEST");
        }
    }
}