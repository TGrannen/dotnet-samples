using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library.Context;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FeatureFlags.LaunchDarkly.Library
{
    class FeatureService : IFeatureService, IJsonFeatureService
    {
        private readonly IServiceProvider _provider;
        private readonly ILdClient _client;
        private readonly Converter _converter;

        public FeatureService(IServiceProvider provider, ILdClient client)
        {
            _provider = provider;
            _client = client;
            _converter = new Converter();
        }

        public Task<bool> IsEnabledAsync(string key, IFeatureContext context = null, bool defaultValue = false)
        {
            var user = _converter.Convert(context ?? GetProvider().GetUser());
            var result = _client.BoolVariation(key, user, defaultValue);
            return Task.FromResult(result);
        }

        public Task<T> GetConfiguration<T>(string key, IFeatureContext context = null, T defaultValue = default) where T : class
        {
            var user = _converter.Convert(context ?? GetProvider().GetUser());
            var json = _client.JsonVariation(key, user, GetGenericDefaultValue(defaultValue));
            return Task.FromResult(JsonConvert.DeserializeObject<T>(json.ToJsonString()));
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