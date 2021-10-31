namespace FeatureFlags.LaunchDarkly.Library.Context
{
    public class CustomContextAttribute<T> : ContextAttribute<T>
    {
        public CustomContextAttribute()
        {
        }

        public CustomContextAttribute(string name, T value, bool @private = false) : base(value, @private)
        {
            Name = name;
        }

        public string Name { get; init; }
    }
}