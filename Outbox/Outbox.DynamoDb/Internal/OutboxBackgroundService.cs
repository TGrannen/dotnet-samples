using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Outbox.DynamoDb.Internal;

internal class OutboxBackgroundService : BackgroundService
{
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly IDynamoDBContext _context;
    private readonly IOutboxMessageSender _sender;

    public OutboxBackgroundService(IDynamoDBContext context, IOutboxMessageSender sender, ILogger<OutboxBackgroundService> logger)
    {
        _logger = logger;
        _context = context;
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _context.ScanAsync<OutboxMessage>(new List<ScanCondition>
                {
                    new("Created", ScanOperator.LessThan, DateTime.Now.AddSeconds(-15))
                }).GetRemainingAsync(stoppingToken);

                if (!messages.Any()) continue;
                _logger.LogInformation("Sending messages from the outbox via background service: {@Messages}", messages);
                await _sender.SendOutboxMessages(messages, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during Outbox processing");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}