using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Context
{
    public interface IFeatureContext
    {
        public string Key { get; set; }
        User Build();
    }
}