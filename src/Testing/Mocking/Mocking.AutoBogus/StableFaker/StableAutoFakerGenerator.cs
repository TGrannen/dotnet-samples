using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bogus;
using Mocking.AutoBogus.StableFaker.TypeGenerators;

namespace Mocking.AutoBogus.StableFaker;

internal static class StableAutoFakerGenerator
{
    private static readonly IStableTypeGenerator[] StableFakersList =
    [
        new StableStringTypeGenerator(),
        new StableIntTypeGenerator(),
        new StableGuidTypeGenerator(),
        new StableDateTypeGenerator(),
        new StableBoolTypeGenerator(),
        new StableDecimalTypeGenerator(),
        new StableDoubleTypeGenerator(),
        new StableTimeSpanTypeGenerator(),
    ];

    private static readonly Dictionary<Type, IStableTypeGenerator> StableFakers = StableFakersList.ToDictionary(x => x.Type, x => x);

    public static object GenerateObject(Type type, string path, StableAutoFakerConfig config)
    {
        if (Nullable.GetUnderlyingType(type) != null)
            type = Nullable.GetUnderlyingType(type);

        var propName = path.Split('.').Last();

        // Ignore list
        if (config.IgnoredProperties.Contains(propName))
            return GetDefault(type);

        // Custom property rule
        if (config.CustomPropertyRules.TryGetValue(propName, out var propRule))
            return propRule(path);

        // Custom type rule
        if (config.CustomTypeRules.TryGetValue(type, out var typeRule))
            return typeRule(path);

        // Register enum faker on-demand
        if (type.IsEnum && !StableFakers.ContainsKey(type))
        {
            StableFakers[type] = new StableEnumTypeGenerator(type);
        }

        // Primitive and known types via IStableFaker registry
        if (StableFakers.TryGetValue(type, out var stableFaker))
        {
            return stableFaker.Generate(path, config);
        }

        // Collections: arrays
        if (type.IsArray)
        {
            return GenerateArray(type, path, config);
        }

        // Collections: IEnumerable<T> and IDictionary<K,V> fallbacks
        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            return GenerateEnumerable(type, path, config);
        }

        // Don't attempt to instantiate interfaces or abstract types
        if (type.IsInterface || type.IsAbstract)
            return GetDefault(type);

        // Complex/nested objects
        var obj = TryCreateInstance(type);
        if (obj == null)
            return GetDefault(type);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0))
        {
            try
            {
                var value = GenerateObject(prop.PropertyType, string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}",
                    config);
                prop.SetValue(obj, value);
            }
            catch
            {
                // ignore property set errors to avoid whole object failing on unexpected types
            }
        }

        return obj;
    }

    private static object GenerateArray(Type type, string path, StableAutoFakerConfig config)
    {
        var elementType = type.GetElementType()!;
        var length = config.DefaultCollectionSize;
        var array = Array.CreateInstance(elementType, length);
        for (var i = 0; i < length; i++)
            array.SetValue(GenerateObject(elementType, $"{path}[{i}]", config), i);
        return array;
    }

    private static object GenerateEnumerable(Type type, string path, StableAutoFakerConfig config)
    {
        if (type.IsGenericType)
        {
            var genDef = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments();

            // IDictionary<K,V>
            if (genDef == typeof(Dictionary<,>) || genDef == typeof(IDictionary<,>))
            {
                return GenerateDictionary(path, config, args);
            }

            // IEnumerable<T> fallback -> List<T>
            var elementType = args[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList?)TryCreateInstance(listType) ?? (IList)Activator.CreateInstance(listType)!;
            for (var i = 0; i < config.DefaultCollectionSize; i++)
            {
                list.Add(GenerateObject(elementType, $"{path}[{i}]", config));
            }

            // If the requested type is an interface/abstract that can accept a List<T>, return the list (it will be assignable to IEnumerable<T>)
            if (type.IsInstanceOfType(list))
            {
                return list;
            }

            // Try to create the concrete requested collection type and populate it if possible
            var requested = TryCreateInstance(type);
            if (requested is not IList requestedList)
            {
                return list;
            }

            foreach (var item in list)
            {
                requestedList.Add(item);
            }

            return requestedList;
        }
        else
        {
            // Non-generic IEnumerable fallback: create a List<object>
            var list = new ArrayList();
            for (var i = 0; i < config.DefaultCollectionSize; i++)
            {
                list.Add(GenerateObject(typeof(object), $"{path}[{i}]", config));
            }

            return list;
        }
    }

    private static object GenerateDictionary(string path, StableAutoFakerConfig config, Type[] args)
    {
        var keyType = args[0];
        var valueType = args[1];
        var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dict = (IDictionary?)TryCreateInstance(dictType) ?? (IDictionary)Activator.CreateInstance(typeof(Hashtable))!;
        for (var i = 0; i < config.DefaultCollectionSize; i++)
        {
            var key = GenerateObject(keyType, $"{path}.Key{i}", config);
            var value = GenerateObject(valueType, $"{path}[{key}]", config);
            dict.Add(key, value);
        }

        return dict;
    }

    public static Faker NewFaker(string seed, StableAutoFakerConfig? config = null)
    {
        var combined = config?.GlobalSeed != null
            ? $"{config.GlobalSeed.Value}:{seed}"
            : seed;

        var bytes = Encoding.UTF8.GetBytes(combined.ToLowerInvariant());
        var hashBytes = MD5.HashData(bytes);
        var stableSeed = BitConverter.ToInt32(hashBytes, 0);

        return new Faker()
        {
            Random = new Randomizer(stableSeed)
        };
    }

    private static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type)! : null!;
    }

    private static object? TryCreateInstance(Type type)
    {
        try
        {
            // Try parameterless constructor
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);
        }
        catch
        {
            // ignore failed activations
        }

        return null;
    }
}