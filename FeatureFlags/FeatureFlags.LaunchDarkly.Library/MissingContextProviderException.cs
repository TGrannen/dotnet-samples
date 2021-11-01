using System;

namespace FeatureFlags.LaunchDarkly.Library
{
    public class MissingContextProviderException : Exception
    {
        public MissingContextProviderException() : base(
            "Context Provider was not found via dependency injection when it was expected. " +
            "Be sure to add an implementation of the IContextProvider during startup.")
        {
        }
    }
}