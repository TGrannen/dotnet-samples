namespace FeatureFlags.Library.Flagsmith;

public interface IEnvFeatureService
{
    Task<bool> IsEnabledAsync(string key);
    Task<bool> IsEnabledAsync(string key, bool defaultValue);
}