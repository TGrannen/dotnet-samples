using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Config;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class LaunchDarklyFeatureService : IFeatureService
    {
        private readonly IFeatureKeyConverter _keyConverter;
        private readonly LdClient _client;

        public LaunchDarklyFeatureService(IFeatureKeyConverter keyConverter, ILaunchDarklyConfig config, ILoggerFactory loggerFactory)
        {
            _keyConverter = keyConverter;
            var ldConfig = Configuration.Builder(config.SdkKey)
                .Logging(LdMicrosoftLogging.Adapter(loggerFactory))
                .Build();
            _client = new LdClient(ldConfig);
        }

        public Task<bool> IsEnabledAsync(Features feature)
        {
            var result = GetBoolVariation(feature);
            return Task.FromResult(result);
        }

        public Task<bool> IsNotEnabledAsync(Features feature)
        {
            var result = GetBoolVariation(feature);
            return Task.FromResult(!result);
        }

        public Task<bool> IsEnabledAsync<TContext>(Features feature, TContext context)
        {
            var result = GetBoolVariation<TContext>(feature);
            return Task.FromResult(result);
        }

        public Task<bool> IsNotEnabledAsync<TContext>(Features feature, TContext context)
        {
            var result = GetBoolVariation<TContext>(feature);
            return Task.FromResult(!result);
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