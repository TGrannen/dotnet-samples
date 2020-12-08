using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthEndpoints.Web.HealthChecks
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMyHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddCheck<RandomHealthCheck>("Random", null, new[] { "Dummy" })
                .AddCheck<RandomHealthCheck>("Random2", null, new[] { "Dummy" })
                .AddCheck<RandomHealthCheck>("Random3", null, new[] { "Dummy" })
                .AddCheck("Inline", () => HealthCheckResult.Degraded("I'm not sure what's going on here"), new[] { "sql" })
                ;

            services
                .AddHealthChecksUI(setup => setup.DisableDatabaseMigrations())
                .AddInMemoryStorage();
            return services;
        }

        public static void MapMyHealthDetails(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecksUI();
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/health/sql", new HealthCheckOptions
            {
                Predicate = c => c.Tags.Contains("sql"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}