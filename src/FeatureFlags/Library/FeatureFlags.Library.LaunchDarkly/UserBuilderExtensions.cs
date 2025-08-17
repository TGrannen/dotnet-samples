using LaunchDarkly.Sdk;

namespace FeatureFlags.Library.LaunchDarkly;

public static class UserBuilderExtensions
{
    public static Context ToContext(this User user)
    {
        var context = Context.Builder(user.Key)
            .Kind("user") // "user" is the default kind, but you can specify explicitly
            .Name(user.Name)
            .Set("email", user.Email) // custom attributes
            .Build();
        return context;
    }
}