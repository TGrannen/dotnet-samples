using Microsoft.Extensions.DependencyInjection;

namespace Mocking.Mediator;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(DependencyInjection));

        return services;
    }
}