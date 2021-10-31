using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Context
{
    public class UserWithNameContext : IFeatureContext
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public User Build()
        {
            var user = User.Builder(Key)
                .Name(Name)
                .Build();
            return user;
        }
    }
}