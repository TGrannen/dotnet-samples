using Dapper.CleanArchitecture.Infrastructure.DataAccess;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISeedService, SeedService>();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<DbContext>();
        services.AddScoped<DbReadContext>();
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<DbContext>());
        services.AddScoped<IDbReadContext>(provider => provider.GetRequiredService<DbReadContext>());

        return services;
    }
}