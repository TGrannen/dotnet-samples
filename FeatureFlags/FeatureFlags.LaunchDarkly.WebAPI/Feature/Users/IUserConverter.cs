using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    public interface IUserConverter
    {
        User Convert(IFeatureContext obj);
    }
}