namespace FeatureFlags.Library.Core.Context;

public interface IFeatureContext
{
    /// <summary>
    /// Required
    /// </summary>
    public string Key { get; }
    public bool Anonymous { get; }
    public ContextAttribute<string> Avatar { get; }
    public ContextAttribute<string> Secondary { get; }
    public ContextAttribute<string> IPAddress { get; }
    public ContextAttribute<string> Country { get; }
    public ContextAttribute<string> FirstName { get; }
    public ContextAttribute<string> LastName { get; }
    public ContextAttribute<string> Name { get; }
    public ContextAttribute<string> Email { get; }
    public List<CustomContextAttribute<string>> CustomContextAttributes { get; }
}