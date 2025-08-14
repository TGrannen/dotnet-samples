using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bogus;

namespace Mocking.AutoBogus.StableFaker;

internal static class StableAutoFakerGenerator
{
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

        // Primitive types
        if (type == typeof(string))
            return StableString(path, config);
        if (type == typeof(int))
            return StableInt(path, config);
        if (type == typeof(Guid))
            return StableGuid(path, config);
        if (type == typeof(DateTime))
            return StableDate(path, config);
        if (type == typeof(bool))
            return StableBool(path, config);
        if (type.IsEnum)
            return StableEnum(type, path, config);
        if (type == typeof(decimal))
            return (decimal)StableInt(path, config) / 10;
        if (type == typeof(double))
            return StableInt(path, config) / 10.0;
        if (type == typeof(TimeSpan))
            return TimeSpan.FromDays(StableInt(path, config) % 365);

        // Collections: arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var length = config.DefaultCollectionSize;
            var array = Array.CreateInstance(elementType, length);
            for (var i = 0; i < length; i++)
                array.SetValue(GenerateObject(elementType, $"{path}[{i}]", config), i);
            return array;
        }

        // Collections: IEnumerable<T> and IDictionary<K,V> fallbacks
        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            if (type.IsGenericType)
            {
                var genDef = type.GetGenericTypeDefinition();
                var args = type.GetGenericArguments();

                // IDictionary<K,V>
                if (genDef == typeof(Dictionary<,>) || genDef == typeof(IDictionary<,>))
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

    // ===== Stable Generators =====
    private static string StableString(string seed, StableAutoFakerConfig config)
    {
        var faker = NewFaker(seed, config);
        var lower = seed.ToLowerInvariant();
        if (lower.Contains("firstname"))
            return faker.Name.FirstName();
        if (lower.Contains("lastname"))
            return faker.Name.LastName();
        if (lower.Contains("fullname"))
            return faker.Name.FullName();
        if (lower.Contains("email"))
            return faker.Internet.Email();
        if (lower.Contains("city"))
            return faker.Address.City();
        if (lower.Contains("street"))
            return faker.Address.StreetAddress();
        if (lower.Contains("phone"))
            return faker.Phone.PhoneNumber();
        if (lower.Contains("company"))
            return faker.Company.CompanyName();

        return faker.Lorem.Word();
    }

    private static int StableInt(string seed, StableAutoFakerConfig config)
    {
        var faker = NewFaker(seed, config);
        return faker.Random.Int(1, 1000);
    }

    private static Guid StableGuid(string seed, StableAutoFakerConfig config)
    {
        var combined = config.GlobalSeed != null
            ? $"{config.GlobalSeed.Value}:{seed}"
            : seed;

        var bytes1 = Encoding.UTF8.GetBytes(combined.ToLowerInvariant());
        var bytes = MD5.HashData(bytes1);
        return new Guid(bytes);
    }

    private static DateTime StableDate(string seed, StableAutoFakerConfig config)
    {
        var faker = NewFaker(seed, config);
        return faker.Date.Past(10);
    }

    private static bool StableBool(string seed, StableAutoFakerConfig config)
    {
        var faker = NewFaker(seed, config);
        return faker.Random.Bool();
    }

    private static object StableEnum(Type enumType, string seed, StableAutoFakerConfig config)
    {
        var faker = NewFaker(seed, config);
        var values = Enum.GetValues(enumType);
        return values.GetValue(faker.Random.Int(0, values.Length - 1));
    }

    private static Faker NewFaker(string seed, StableAutoFakerConfig? config = null)
    {
        var stableSeed = StableHashInt(seed, config);
        return new Faker()
        {
            Random = new Randomizer(stableSeed)
        };
    }

    private static int StableHashInt(string input, StableAutoFakerConfig? config)
    {
        var combined = config?.GlobalSeed != null
            ? $"{config.GlobalSeed.Value}:{input}"
            : input;

        var bytes = Encoding.UTF8.GetBytes(combined.ToLowerInvariant());
        var hashBytes = MD5.HashData(bytes);
        return BitConverter.ToInt32(hashBytes, 0);
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