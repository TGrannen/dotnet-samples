using System;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys
{
    class FeatureKeyConverter : IFeatureKeyConverter
    {
        public string ConvertToKey(Features feature)
        {
            return feature switch
            {
                Features.Feature1 => "demo-sample-feature",
                Features.Feature2 => "demo-sample-feature-2",
                Features.Feature3 => "demo-json-feature",
                _ => throw new ArgumentOutOfRangeException(nameof(feature), feature, null)
            };
        }
    }
}