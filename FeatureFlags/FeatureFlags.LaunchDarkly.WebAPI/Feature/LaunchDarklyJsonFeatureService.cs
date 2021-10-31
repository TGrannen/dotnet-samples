using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Extensions;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class LaunchDarklyJsonFeatureService : IJsonFeatureService
    {
        private readonly IFeatureKeyConverter _keyConverter;
        private readonly IUserConverter _userConverter;
        private readonly IUserProvider _userProvider;
        private readonly ILdClient _client;

        public LaunchDarklyJsonFeatureService(IFeatureKeyConverter keyConverter,
            IUserConverter userConverter,
            IUserProvider userProvider,
            ILdClient client)
        {
            _keyConverter = keyConverter;
            _userConverter = userConverter;
            _userProvider = userProvider;
            _client = client;
        }

        public Task<T> GetFeatureConfigurationAsync<T>(Features feature)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.JsonVariation<T>(key, _userProvider.GetUser(), new LdValue());
            return Task.FromResult(result);
        }

        public Task<T> GetFeatureConfigurationAsync<T>(Features feature, IFeatureContext context)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.JsonVariation<T>(key, _userConverter.Convert(context), new LdValue());
            return Task.FromResult(result);
        }
    }
}