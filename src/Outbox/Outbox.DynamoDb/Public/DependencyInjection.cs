using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Outbox.DynamoDb.Internal;
using Outbox.DynamoDb.Internal.Background;
using Outbox.DynamoDb.Internal.Sending;

namespace Outbox.DynamoDb;

public static class DependencyInjection
{
    public static IServiceCollection AddDynamoDbOutbox(this IServiceCollection services)
    {
        services.TryAddScoped<IDynamoDbTransaction, DynamoDbTransaction>();
        services.TryAddTransient<IOutboxMessageSender, OutboxMessageSender>();
        services.TryAddSingleton<IOutboxMessageQueue, OutboxMessageQueue>();
        services.AddHostedService<SenderBackgroundService>();

        return services;
    }

    public static IServiceCollection AddDynamoDbOutboxWorkerService(this IServiceCollection services)
    {
        services.TryAddTransient<IOutboxMessageSender, OutboxMessageSender>();
        services.AddHostedService<PurgeOutboxBackgroundService>();

        return services;
    }
}