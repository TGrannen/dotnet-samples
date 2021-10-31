namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys
{
    public interface IFeatureKeyConverter
    {
        string ConvertToKey(Features feature);
    }
}