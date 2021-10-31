using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;
using Newtonsoft.Json;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature
{
    class FeatureService : IFeatureService,IJsonFeatureService
    {
        private readonly IUserProvider _provider;
        private readonly ILdClient _client;

        public FeatureService(IUserProvider provider, ILdClient client)
        {
            _provider = provider;
            _client = client;
        }

        public Task<bool> IsEnabledAsync(string key, IFeatureContext context = null)
        {
            var user = context == null ? _provider.GetUser() : context.Build();
            var result = _client.BoolVariation(key, user);
            return Task.FromResult(result);
        }

        public Task<T> GetJsonConfiguration<T>(string key, IFeatureContext context = null)
        {
            var user = context == null ? _provider.GetUser() : context.Build();
            var json = _client.JsonVariation(key, user, new LdValue());
            return Task.FromResult(JsonConvert.DeserializeObject<T>(json.ToJsonString()));
        }
    }
}