namespace ApiService.Api.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddPersistence(connectionString);
        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services,
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditingSaveChangesInterceptor>();
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>());
            optionsAction?.Invoke(serviceProvider, options);
        });

        return services;
    }

    public static IHostApplicationBuilder AddPersistenceInstrumentation(this IHostApplicationBuilder builder)
    {
        builder.EnrichSqlServerDbContext<ApplicationDbContext>();

        return builder;
    }
}
