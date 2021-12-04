using System.Collections.Generic;

namespace FeatureFlags.Library.Core.Context
{
    public class FeatureContext : IFeatureContext
    {
        public string Key { get; init; }
        public bool Anonymous { get; init; }
        public ContextAttribute<string> Avatar { get; init; }
        public ContextAttribute<string> Secondary { get; init; }
        public ContextAttribute<string> IPAddress { get; init; }
        public ContextAttribute<string> Country { get; init; }
        public ContextAttribute<string> FirstName { get; init; }
        public ContextAttribute<string> LastName { get; init; }
        public ContextAttribute<string> Name { get; init; }
        public ContextAttribute<string> Email { get; init; }
        public List<CustomContextAttribute<string>> CustomContextAttributes { get; init; }
    }
}