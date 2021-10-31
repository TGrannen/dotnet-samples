namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Config
{
    class LaunchDarklyConfig : ILaunchDarklyConfig
    {
        public string SdkKey { get; set; }
    }
}