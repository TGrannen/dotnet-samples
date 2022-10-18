namespace IntegrationTesting.WebAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}