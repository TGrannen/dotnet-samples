namespace FeatureFlags.Library.Core.Context;

public interface IContextProvider
{
    Task<IFeatureContext> GetUserAsync();
}