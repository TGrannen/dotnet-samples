using FeatureFlags.LaunchDarkly.WebAPI.Models;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services
{
    public interface IUserService
    {
        TestUser GetUser();
    }
}