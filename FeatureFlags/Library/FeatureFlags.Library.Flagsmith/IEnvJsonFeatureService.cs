namespace FeatureFlags.Library.Flagsmith;

public interface IEnvJsonFeatureService
{
    Task<T> GetConfiguration<T>(string key) where T : class;
    Task<T> GetConfiguration<T>(string key, T defaultValue) where T : class;
}