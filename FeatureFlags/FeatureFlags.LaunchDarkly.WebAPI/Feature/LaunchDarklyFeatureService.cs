using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class LaunchDarklyFeatureService : IBoolFeatureService
    {
        private readonly IFeatureKeyConverter _keyConverter;
        private readonly ILdClient _client;

        public LaunchDarklyFeatureService(IFeatureKeyConverter keyConverter, ILdClient client)
        {
            _keyConverter = keyConverter;
            _client = client;
        }

        public Task<bool> IsEnabledAsync(Features feature)
        {
            var result = GetBoolVariation(feature);
            return Task.FromResult(result);
        }
        
        public Task<bool> IsEnabledAsync<TContext>(Features feature, TContext context)
        {
            var result = GetBoolVariation<TContext>(feature);
            return Task.FromResult(result);
        }
        
        private bool GetBoolVariation(Features feature)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.BoolVariation(key, User.WithKey("TEST"));
            return result;
        }

        private bool GetBoolVariation<TContext>(Features feature)
        {
            var key = _keyConverter.ConvertToKey(feature);
            var result = _client.BoolVariation(key, User.WithKey("TEST"));
            return result;
        }
    }
}