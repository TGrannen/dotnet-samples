using FeatureFlags.LaunchDarkly.WebAPI.Features;
using FeatureFlags.Library.LaunchDarkly;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services;

/// <summary>
/// Simply exists to show exactly what code is required to access feature flags with the LaunchDarkly SDK 
/// </summary>
public class LaunchDarklyDirectService
{
    private readonly ILdClient _client;

    public LaunchDarklyDirectService(ILdClient client)
    {
        _client = client;
    }

    public bool IsSampleOneEnabled()
    {
        return _client.BoolVariation("demo-sample-feature", User.WithKey("TEST").ToContext());
    }

    public bool IsSampleTwoEnabled(TestUser user)
    {
        var contextUser = User.Builder(user.Id).Name(user.Name).Build();
        return _client.BoolVariation("demo-sample-feature-2", contextUser.ToContext());
    }

    public LdValue JsonSample(TestUser user)
    {
        var contextUser = User.Builder(user.Id).Name(user.Name).Build();
        return _client.JsonVariation("demo-json-feature", contextUser.ToContext(), LdValue.Null);
    }

    public bool IsSampleOneEnabledCustom()
    {
        var builder = User.Builder("TEST");

        builder.Custom("My Data Stuff", "My fancy value");

        var contextUser = builder.Build();
        return _client.BoolVariation("demo-sample-feature", new Context());
    }
}