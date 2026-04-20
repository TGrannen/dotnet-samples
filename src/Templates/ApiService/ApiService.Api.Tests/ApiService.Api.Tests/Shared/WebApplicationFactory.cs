using ApiService.Api.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.AspNetCore;

namespace ApiService.Api.Tests.Shared;

public class WebApplicationFactory : TestWebApplicationFactory<Program>
{
    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    public PostgresContainer Postgres { get; init; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", Postgres.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            // 1. Register the tracker so it can be injected
            services.AddSingleton<SaveChangesTracker>();

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddPersistence(Postgres.GetConnectionString(), (sp, options) =>
            {
                var tracker = sp.GetRequiredService<SaveChangesTracker>();
                options.AddInterceptors(new SaveCountInterceptor(tracker));
            });
        });
    }
}
