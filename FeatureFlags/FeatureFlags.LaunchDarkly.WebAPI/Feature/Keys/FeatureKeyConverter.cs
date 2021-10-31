using System;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Keys
{
    class FeatureKeyConverter : IFeatureKeyConverter
    {
        public string ConvertToKey(Features feature)
        {
            switch (feature)
            {
                case Features.Feature1:
                    return "demo-sample-feature";
                default:
                    throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
            }
        }
    }
}