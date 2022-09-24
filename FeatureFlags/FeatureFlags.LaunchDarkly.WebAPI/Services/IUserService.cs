using FeatureFlags.LaunchDarkly.WebAPI.Features;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services;

public interface IUserService
{
    TestUser GetUser();
}