using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Outbox.DynamoDb.Internal;

namespace Outbox.DynamoDb;

public static class DependencyInjection
{
    public static IServiceCollection AddDynamoDbOutbox(this IServiceCollection services)
    {
        services.TryAddScoped<IDynamoDbTransaction, DynamoDbTransaction>();

        return services;
    }

    public static IServiceCollection AddDynamoDbOutboxWorkerService(this IServiceCollection services)
    {
        services.AddHostedService<OutboxBackgroundService>();

        return services;
    }
}