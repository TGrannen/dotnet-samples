using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;
using Newtonsoft.Json;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Extensions
{
    public static class LdClientExtensions
    {
        public static T JsonVariation<T>(this ILdClient ldClient, string key, User user, LdValue? defaultValue = null)
        {
            var result = ldClient.JsonVariation(key, user, defaultValue == null ? LdValue.Null : new LdValue());
            return JsonConvert.DeserializeObject<T>(result.ToJsonString());
        }
    }
}