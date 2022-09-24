namespace Configuration.Web.Providers.CustomProvider;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEntityConfiguration(this IConfigurationBuilder builder)
    {
        builder.Add(new CustomConfigurationSource
        {
            ReloadOnChange = true
        });
        return builder;
    }
}