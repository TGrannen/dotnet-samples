using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.Library
{
    public interface IFeatureContext
    {
        public string Key { get; set; }
        User Build();
    }
}