using Microsoft.Extensions.Hosting;

namespace Outbox.DynamoDb.Internal;

internal class OutboxBackgroundService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}