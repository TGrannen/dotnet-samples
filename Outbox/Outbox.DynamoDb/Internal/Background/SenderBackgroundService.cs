using Amazon.DynamoDBv2.DocumentModel;
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
        var messages = new List<OutboxMessage>(100);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = _messageQueue.TryDequeue();
                while (message != null)
                {
                    messages.Add(message);
                    message = _messageQueue.TryDequeue();
                }

                if (!messages.Any())
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }

                await _sender.SendOutboxMessages(messages, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during Outbox processing");
            }
            finally
            {
                messages.Clear();
                await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
            }
        }
    }
}