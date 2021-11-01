namespace FeatureFlags.LaunchDarkly.Library.Context
{
    public class ContextAttribute<T>
    {
        public ContextAttribute()
        {
        }

        public ContextAttribute(T value, bool @private = false)
        {
            Value = value;
            Private = @private;
        }

        public bool Private { get; init; }
        public T Value { get; init; }
    }
}