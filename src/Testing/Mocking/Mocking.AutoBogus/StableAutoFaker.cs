using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bogus;

namespace Mocking.AutoBogus;

public static class StableAutoFaker
{
    public static int DefaultCollectionSize { get; set; } = 3;

    public static T Generate<T>() where T : class, new()
    {
        return (T)GenerateObject(typeof(T), "");
    }

    public static List<T> GenerateList<T>(int count) where T : class, new()
    {
        return Enumerable.Range(0, count).Select(_ => Generate<T>()).ToList();
    }

    private static object GenerateObject(Type type, string path)
    {
        if (Nullable.GetUnderlyingType(type) != null)
            type = Nullable.GetUnderlyingType(type);

        if (type == typeof(string))
            return StableString(path);
        if (type == typeof(int))
            return StableInt(path);
        if (type == typeof(Guid))
            return StableGuid(path);
        if (type == typeof(DateTime))
            return StableDate(path);
        if (type == typeof(bool))
            return StableBool(path);
        if (type.IsEnum)
            return StableEnum(type, path);

        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            if (type.IsGenericType)
            {
                var elementType = type.GetGenericArguments()[0];

                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var keyType = type.GetGenericArguments()[0];
                    var valueType = type.GetGenericArguments()[1];
                    var dict = (IDictionary)Activator.CreateInstance(type);
                    for (int i = 0; i < DefaultCollectionSize; i++)
                    {
                        var key = GenerateObject(keyType, $"{path}.Key{i}");
                        var value = GenerateObject(valueType, $"{path}[{key}]");
                        dict.Add(key, value);
                    }

                    return dict;
                }

                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                for (int i = 0; i < DefaultCollectionSize; i++)
                {
                    list.Add(GenerateObject(elementType, $"{path}[{i}]"));
                }

                return list;
            }
        }

        var obj = Activator.CreateInstance(type);
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanWrite))
        {
            var value = GenerateObject(prop.PropertyType, $"{path}.{prop.Name}");
            prop.SetValue(obj, value);
        }

        return obj;
    }

    private static string StableString(string seed)
    {
        var faker = NewFaker(seed);
        var lower = seed.ToLower();
        if (lower.Contains("firstname"))
        {
            return faker.Name.FirstName();
        }

        if (lower.Contains("lastname"))
        {
            return faker.Name.LastName();
        }

        if (lower.Contains("fullname"))
        {
            return faker.Name.FullName();
        }

        if (lower.Contains("email"))
        {
            return faker.Internet.Email();
        }

        if (lower.Contains("city"))
        {
            return faker.Address.City();
        }

        if (lower.Contains("street"))
        {
            return faker.Address.StreetAddress();
        }

        if (lower.Contains("phone"))
        {
            return faker.Phone.PhoneNumber();
        }

        if (lower.Contains("company"))
        {
            return faker.Company.CompanyName();
        }

        return faker.Lorem.Word();
    }

    private static int StableInt(string seed)
    {
        var faker = NewFaker(seed);
        return faker.Random.Int(1, 1000);
    }

    private static Guid StableGuid(string seed)
    {
        return GuidFromSeed(seed);
    }

    private static DateTime StableDate(string seed)
    {
        var faker = NewFaker(seed);
        return faker.Date.Past(10);
    }

    private static bool StableBool(string seed)
    {
        var faker = NewFaker(seed);
        return faker.Random.Bool();
    }

    private static object StableEnum(Type enumType, string seed)
    {
        var faker = NewFaker(seed);
        var values = Enum.GetValues(enumType);
        return values.GetValue(faker.Random.Int(0, values.Length - 1));
    }

    public static Faker NewFaker(string seed)
    {
        var stableSeed = StableHash(seed);
        return new Faker()
        {
            Random = new Randomizer(stableSeed)
        };
    }

    private static int StableHash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(bytes);
        return BitConverter.ToInt32(hashBytes, 0);
    }

    private static Guid GuidFromSeed(string seed)
    {
        var bytes = new byte[16];
        var hash = StableHash(seed);
        BitConverter.GetBytes(hash).CopyTo(bytes, 0);
        BitConverter.GetBytes(~hash).CopyTo(bytes, 4);
        return new Guid(bytes);
    }
}