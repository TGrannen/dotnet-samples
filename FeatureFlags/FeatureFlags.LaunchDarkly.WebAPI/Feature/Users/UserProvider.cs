using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    class UserProvider : IUserProvider
    {
        public User GetUser()
        {
            return User.WithKey("TEST");
        }
    }
}