namespace FeatureFlags.Library.Flagsmith.Internal;

public class Params
{
    public string Key { get; init; }
    public List<Trait>? Traits { get; init; }
}

public class Converter
{
    public Params Convert(IFeatureContext context)
    {
        if (context.Key == null)
        {
            throw new ArgumentException("Context is missing Key");
        }

        var attributes = new Dictionary<string, object>();

        ApplyContextAttribute(context.Avatar, "Avatar", attributes);
        ApplyContextAttribute(context.Secondary, "Secondary", attributes);
        ApplyContextAttribute(context.IPAddress, "IPAddress", attributes);
        ApplyContextAttribute(context.Country, "Country", attributes);
        ApplyContextAttribute(context.FirstName, "FirstName", attributes);
        ApplyContextAttribute(context.LastName, "LastName", attributes);
        ApplyContextAttribute(context.Name, "Name", attributes);
        ApplyContextAttribute(context.Email, "Email", attributes);

        ApplyCustomAttributes(context, attributes);

        return new Params
        {
            Key = context.Key,
            Traits = !attributes.Any() ? null : attributes.Select(x => new Trait(x.Key, x.Value)).ToList()
        };
    }

    private static void ApplyContextAttribute<T>(ContextAttribute<T>? attr, string name, IDictionary<string, object> dictionary)
    {
        if (attr is null or { Value: null })
        {
            return;
        }

        dictionary.Add(name, attr.Value);
    }

    private static void ApplyCustomAttributes(IFeatureContext context, IDictionary<string, object> dictionary)
    {
        if (context.CustomContextAttributes == null || !context.CustomContextAttributes.Any())
        {
            return;
        }

        foreach (var attribute in context.CustomContextAttributes)
        {
            dictionary.Add(attribute.Name, attribute.Value);
        }
    }
}