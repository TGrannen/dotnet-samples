using System;
using System.Linq;
using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.Library.Context
{
    public class Converter
    {
        public User Convert(IFeatureContext context)
        {
            if (context.Key == null)
            {
                throw new ArgumentException("Context is missing Key");
            }

            var builder = User.Builder(context.Key).Anonymous(context.Anonymous);

            ApplyContextAttribute(context.Avatar, attr => builder.Avatar(attr.Value));
            ApplyContextAttribute(context.Secondary, attr => builder.Secondary(attr.Value));
            ApplyContextAttribute(context.IPAddress, attr => builder.IPAddress(attr.Value));
            ApplyContextAttribute(context.Country, attr => builder.Country(attr.Value));
            ApplyContextAttribute(context.FirstName, attr => builder.FirstName(attr.Value));
            ApplyContextAttribute(context.LastName, attr => builder.LastName(attr.Value));
            ApplyContextAttribute(context.Name, attr => builder.Name(attr.Value));
            ApplyContextAttribute(context.Email, attr => builder.Email(attr.Value));

            ApplyCustomAttributes(context, builder);

            return builder.Build();
        }

        private static void ApplyContextAttribute<T>(ContextAttribute<T> attr,
            Func<ContextAttribute<T>, IUserBuilderCanMakeAttributePrivate> func)
        {
            if (attr == null)
            {
                return;
            }

            var privateAttr = func(attr);
            if (attr.Private)
            {
                privateAttr.AsPrivateAttribute();
            }
        }

        private static void ApplyCustomAttributes(IFeatureContext context, IUserBuilder builder)
        {
            if (context.CustomContextAttributes == null || !context.CustomContextAttributes.Any())
            {
                return;
            }

            foreach (var attribute in context.CustomContextAttributes)
            {
                var privateAttr = builder.Custom(attribute.Name, attribute.Value);
                if (attribute.Private)
                {
                    privateAttr.AsPrivateAttribute();
                }
            }
        }
    }
}