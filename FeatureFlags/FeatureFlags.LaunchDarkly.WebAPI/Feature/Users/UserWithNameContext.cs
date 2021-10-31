namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    public class UserWithNameContext : IFeatureContext
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}