using Configuration.Web.Providers.CustomProvider;

namespace Configuration.Web.Extensions;

public static class Extensions
{
    public static OptionsBuilder<TC> ConfigureValidated<TC>(this IServiceCollection services, IConfiguration configuration,
        bool onStart = true) where TC : class
    {
        var optionsBuilder = services.AddOptions<TC>().Bind(configuration);

        optionsBuilder.Services.AddTransient<IValidateOptions<TC>>(sp =>
            new FluentValidationOptions<TC>(optionsBuilder.Name, sp.GetServices<IValidator<TC>>()));

        if (onStart)
        {
            optionsBuilder.ValidateOnStart();
        }

        return optionsBuilder;
    }

    public static IConfigurationBuilder AddCustomConfiguration(this IConfigurationBuilder builder)
    {
        builder.Add(new CustomConfigurationSource { ReloadOnChange = true });
        return builder;
    }
}