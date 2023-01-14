namespace FeatureFlags.Library.Flagsmith.Internal;

internal interface IEnvFlagsProvider
{
    Task<Flags> GetFlags();
}