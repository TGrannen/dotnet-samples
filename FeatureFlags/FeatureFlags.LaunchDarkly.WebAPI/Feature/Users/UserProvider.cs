using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    public interface IUserProvider
    {
        User GetUser();
    }

    class UserProvider : IUserProvider
    {
        public User GetUser()
        {
            return User.WithKey("TEST");
        }
    }
}