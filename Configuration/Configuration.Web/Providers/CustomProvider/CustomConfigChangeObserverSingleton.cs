namespace Configuration.Web.Providers.CustomProvider;

public class CustomConfigChangeObserverSingleton
{
    public event EventHandler<ConfigChangeEventArgs> Changed;

    public void OnChanged(ConfigChangeEventArgs e)
    {
        ThreadPool.QueueUserWorkItem((_) => Changed?.Invoke(this, e));
    }

    #region Singleton

    private static readonly Lazy<CustomConfigChangeObserverSingleton> lazy = new(() => new CustomConfigChangeObserverSingleton());

    private CustomConfigChangeObserverSingleton()
    {
    }

    public static CustomConfigChangeObserverSingleton Instance => lazy.Value;

    #endregion Singleton
}