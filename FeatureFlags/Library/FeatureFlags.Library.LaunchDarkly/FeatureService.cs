using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureFlags.Library.Core;
using FeatureFlags.Library.Core.Context;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FeatureFlags.Library.LaunchDarkly
{
    class FeatureService : IFeatureService, IJsonFeatureService
    {
        private readonly IServiceProvider _provider;
        private readonly ILdClient _client;
        private readonly Converter _converter;

        public FeatureService(IServiceProvider provider, ILdClient client, Converter converter)
        {
            _provider = provider;
            _client = client;
            _converter = converter;
        }

        public async Task<bool> IsEnabledAsync(string key, IFeatureContext context = null, bool defaultValue = false)
        {
            var user = _converter.Convert(context ?? await GetProvider().GetUserAsync());
            var result = _client.BoolVariation(key, user, defaultValue);
            return result;
        }

        public async Task<T> GetConfiguration<T>(string key, IFeatureContext context = null, T defaultValue = default) where T : class
        {
            var user = _converter.Convert(context ?? await GetProvider().GetUserAsync());
            var json = _client.JsonVariation(key, user, GetGenericDefaultValue(defaultValue));
            return JsonConvert.DeserializeObject<T>(json.ToJsonString());
        }

        private IContextProvider GetProvider()
        {
            var contextProvider = _provider.GetService<IContextProvider>();
            if (contextProvider == null)
            {
                throw new MissingContextProviderException();
            }

            return contextProvider;
        }

        private static LdValue GetGenericDefaultValue<T>(T defaultValue) where T : class
        {
            var ldDefault = EqualityComparer<T>.Default.Equals(defaultValue, default)
                ? LdValue.Null
                : LdValue.Parse(JsonConvert.SerializeObject(defaultValue));
            return ldDefault;
        }
    }
}