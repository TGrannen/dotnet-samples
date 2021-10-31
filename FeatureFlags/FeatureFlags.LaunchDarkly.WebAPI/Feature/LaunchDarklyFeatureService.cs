using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class LaunchDarklyFeatureService : IBoolFeatureService
    {
        private readonly IFeatureKeyConverter _keyConverter;
        private readonly IUserConverter _userConverter;
        private readonly ILdClient _client;

        public LaunchDarklyFeatureService(IFeatureKeyConverter keyConverter, IUserConverter userConverter, ILdClient client)
        {
            _keyConverter = keyConverter;
            _userConverter = userConverter;
            _client = client;
        }

        public Task<bool> IsEnabledAsync(Features feature)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.BoolVariation(key, User.WithKey("TEST"));
            return Task.FromResult(result);
        }

        public Task<bool> IsEnabledAsync(Features feature, IFeatureContext context)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.BoolVariation(key, _userConverter.Convert(context));
            return Task.FromResult(result);
        }
    }
}