using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Outbox.DynamoDb.Internal.Sending;

namespace Outbox.DynamoDb.Internal.Background;

internal class SenderBackgroundService : BackgroundService
{
    private readonly IOutboxMessageQueue _messageQueue;
    private readonly ILogger<SenderBackgroundService> _logger;
    private readonly IOutboxMessageSender _sender;

    public SenderBackgroundService(IOutboxMessageQueue messageQueue, IOutboxMessageSender sender, ILogger<SenderBackgroundService> logger)
    {
        _messageQueue = messageQueue;
        _logger = logger;
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_messageQueue.IsEmpty())
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }

                var messages = _messageQueue.Take();
                await _sender.SendOutboxMessages(messages, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during Outbox Queue processing");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
            }
        }
    }
}