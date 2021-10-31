namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Config
{
    public interface ILaunchDarklyConfig
    {
        string SdkKey { get; set; }
    }
}