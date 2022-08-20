using Dapper.CleanArchitecture.Infrastructure.DataAccess;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;
using Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<SeedService>();

        services.AddScoped<DbContext>();
        services.AddScoped<DbReadContext>();
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<DbContext>());
        services.AddScoped<IDbReadContext>(provider => provider.GetRequiredService<DbReadContext>());

        services.AddScoped<IDbConnection>(provider =>
        {
            var service = provider.GetRequiredService<IDbConnectionStringProvider>();
            var logger = provider.GetRequiredService<ILogger<DbContext>>();
            logger.LogDebug("Opening a new DB Connection");
            var connection = new NpgsqlConnection(service.GetConnectionString());
            connection.Open();
            return connection;
        });

        return services;
    }
}