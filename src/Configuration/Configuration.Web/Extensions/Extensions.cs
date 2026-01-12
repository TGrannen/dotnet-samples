using Configuration.Web.Providers;

namespace Configuration.Web.Extensions;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public OptionsBuilder<TC> ConfigureValidated<TC>(IConfiguration configuration, bool onStart = true) where TC : class
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

        public IServiceCollection AddCustomRuntimeConfiguration(IConfigurationBuilder builder)
        {
            services.AddTransient<ICustomRuntimeConfiguration, CustomRuntimeConfiguration>();
            builder.Add(new CustomRuntimeConfigurationSource());
            return services;
        }
    }
}