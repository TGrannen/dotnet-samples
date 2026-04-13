using System.Reflection;
using ApiService.Api.Common.Behaviors;
using Microsoft.AspNetCore.OpenApi;

namespace ApiService.Api.Common;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }

        public IServiceCollection AddOpenApiWithFullNames()
        {
            services.AddOpenApi(options =>
            {
                options.CreateSchemaReferenceId = static jsonTypeInfo =>
                {
                    const string featuresMarker = ".Features.";
                    var fullName = jsonTypeInfo.Type.FullName;
                    if (string.IsNullOrEmpty(fullName))
                    {
                        return OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
                    }

                    var i = fullName.IndexOf(featuresMarker, StringComparison.Ordinal);
                    if (i < 0)
                    {
                        return OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
                    }

                    var tail = fullName[(i + featuresMarker.Length)..];
                    return tail.Replace('+', '.');
                };
            });

            return services;
        }
    }
}
