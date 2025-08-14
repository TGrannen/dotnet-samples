using System.Collections.Immutable;

namespace Mocking.AutoBogus.StableFaker;

public class StableAutoFakerConfig
{
    public IImmutableDictionary<string, Func<string, object>> CustomPropertyRules => _customPropertyRules.ToImmutableDictionary();
    public IImmutableDictionary<Type, Func<string, object>> CustomTypeRules => _customTypeRules.ToImmutableDictionary();
    public HashSet<string> IgnoredProperties => _ignoredProperties.ToHashSet();

    private readonly Dictionary<string, Func<string, object>> _customPropertyRules = new();
    private readonly Dictionary<Type, Func<string, object>> _customTypeRules = new();
    private readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase);
    public int? GlobalSeed { get; private set; }
    public int DefaultCollectionSize { get; private set; } = 3;

    public StableAutoFakerConfig WithPropertyRule(string propertyName, Func<string, object> generator)
    {
        _customPropertyRules[propertyName] = generator;
        return this;
    }

    public StableAutoFakerConfig WithTypeRule<T>(Func<string, object> generator)
    {
        _customTypeRules[typeof(T)] = generator;
        return this;
    }

    public StableAutoFakerConfig Ignore(string propertyName)
    {
        _ignoredProperties.Add(propertyName);
        return this;
    }

    public StableAutoFakerConfig WithCollectionSize(int size)
    {
        DefaultCollectionSize = size;
        return this;
    }

    public StableAutoFakerConfig WithGlobalSeed(int seed)
    {
        GlobalSeed = seed;
        return this;
    }

    public StableAutoFakerConfig Reset()
    {
        _customPropertyRules.Clear();
        _customTypeRules.Clear();
        _ignoredProperties.Clear();
        DefaultCollectionSize = 3;
        GlobalSeed = null;
        return this;
    }
}