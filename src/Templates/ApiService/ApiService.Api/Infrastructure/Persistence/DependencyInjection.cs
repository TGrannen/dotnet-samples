using Microsoft.EntityFrameworkCore;

namespace ApiService.Api.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditingSaveChangesInterceptor>();
        services.AddDbContextPool<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>());
        });

        return services;
    }

    public static IHostApplicationBuilder AddPersistenceInstrumentation(this IHostApplicationBuilder builder)
    {
        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        return builder;
    }
}
