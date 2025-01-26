namespace Configuration.Web.Providers;

public interface ICustomRuntimeConfiguration
{
    void SetValue(string key, string value)
    {
        CustomRuntimeConfigurationProvider.Instance.SetValue(key, value);
    }
}

public class CustomRuntimeConfiguration : ICustomRuntimeConfiguration
{
}

public class CustomRuntimeConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return CustomRuntimeConfigurationProvider.Instance;
    }
}

public class CustomRuntimeConfigurationProvider : ConfigurationProvider
{
    private readonly Dictionary<string, string> _localData = new();

    public override void Load()
    {
        Data = _localData;
    }

    public void SetValue(string key, string value)
    {
        _localData[key] = value;
        Load();
    }

    private static readonly Lazy<CustomRuntimeConfigurationProvider> Lazy = new(() => new CustomRuntimeConfigurationProvider());
    public static CustomRuntimeConfigurationProvider Instance => Lazy.Value;

    private CustomRuntimeConfigurationProvider()
    {
    }
}