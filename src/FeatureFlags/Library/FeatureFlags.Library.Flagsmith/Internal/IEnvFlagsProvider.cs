namespace FeatureFlags.Library.Flagsmith.Internal;

internal interface IEnvFlagsProvider
{
    Task<IFlags> GetFlags();
}