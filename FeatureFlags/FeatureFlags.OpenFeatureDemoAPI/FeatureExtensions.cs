namespace FeatureFlags.OpenFeatureDemoAPI;

public static class FeatureExtensions
{
    public static Flag<bool> GenerateConfigurationBoolFlag(this IConfiguration configuration, string configSetting)
    {
        return new Flag<bool>(new Dictionary<string, bool>
        {
            { false.ToString(), false },
            { true.ToString(), true }
        }, configuration.GetValue<bool>(configSetting).ToString(), _ => configuration.GetValue<bool>(configSetting).ToString());
    }

    public static Flag<bool> GenerateConfigurationFlag<T>(this IConfiguration configuration,
        string configSetting,
        Func<EvaluationContext, T?, bool> contextEvaluator)
    {
        return new Flag<bool>(new Dictionary<string, bool>
        {
            { false.ToString(), false },
            { true.ToString(), true }
        }, false.ToString(), context => contextEvaluator(context, configuration.GetValue<T>(configSetting)).ToString());
    }
}