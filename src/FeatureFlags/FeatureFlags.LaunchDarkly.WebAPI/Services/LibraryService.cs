using FeatureFlags.LaunchDarkly.WebAPI.Features;
using FeatureFlags.LaunchDarkly.WebAPI.Features.Models;
using FeatureFlags.Library.Core;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services;

/// <summary>
/// Shows how to use the abstracted Library project to query LaunchDarkly Feature Flags 
/// </summary>
public class LibraryService
{
    private readonly IFeatureService _featureService;
    private readonly IJsonFeatureService _jsonFeatureService;

    public LibraryService(IFeatureService featureService, IJsonFeatureService jsonFeatureService)
    {
        _featureService = featureService;
        _jsonFeatureService = jsonFeatureService;
    }

    public async Task<bool> IsSampleOneEnabled()
    {
        return await _featureService.IsEnabledAsync("demo-sample-feature");
    }

    public async Task<bool> IsSampleTwoEnabled(TestUser user)
    {
        return await _featureService.IsEnabledAsync("demo-sample-feature-2", new FeatureContext
        {
            Key = user.Id,
            Name = new ContextAttribute<string>(user.Name)
        });
    }

    public async Task<Feature3Dto> JsonSample(TestUser user)
    {
        return await _jsonFeatureService.GetConfiguration<Feature3Dto>("demo-json-feature",
            new FeatureContext
            {
                Key = user.Id,
                Name = new ContextAttribute<string>(user.Name)
            });
    }

    public async Task<bool> IsSampleOneEnabledCustom()
    {
        return await _featureService.IsEnabledAsync("demo-sample-feature", new FeatureContext
        {
            Key = "TEST",
            CustomContextAttributes = new List<CustomContextAttribute<string>>
            {
                new("My Data Stuff", "My fancy value")
            }
        });
    }
}