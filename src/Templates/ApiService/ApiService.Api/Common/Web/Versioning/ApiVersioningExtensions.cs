using System.Reflection;
using Asp.Versioning;

namespace ApiService.Api.Common.Web.Versioning;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = SupportedApiVersions.V1;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader(SupportedApiVersions.QueryParameterName),
                    new HeaderApiVersionReader(SupportedApiVersions.HeaderName));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = false;
            });

        return services;
    }

    public static void MapVersionedApiEndpoints(this IEndpointRouteBuilder app, Assembly assembly)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(SupportedApiVersions.V1)
            .ReportApiVersions()
            .Build();

        var api = app.MapGroup("api")
            .WithApiVersionSet(versionSet);

        var endpointTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(api);
        }
    }
}
