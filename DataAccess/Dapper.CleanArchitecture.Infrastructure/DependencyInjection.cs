using Dapper.CleanArchitecture.Domain.Common.Interfaces;
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
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<DbContext>());
        services.AddScoped<IDbReadContext>(provider => provider.GetRequiredService<DbContext>());

        services.AddScoped<IDbConnection>(provider =>
        {
            var service = provider.GetRequiredService<IDbConnectionStringProvider>();
            var connection = new NpgsqlConnection(service.GetConnectionString());
            connection.Open();
            return connection;
        });

        return services;
    }
}