using System.Text.Json.Serialization;

namespace KafkaFlowSample.Consumer.Middleware;

internal class CdcBatchProcessingMiddleware(ILogger<CdcBatchProcessingMiddleware> logger) : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();

        var messages = batch.Select(ctx => (DebeziumMessage<CdcTableModel>)ctx.Message.Value).ToList();

        logger.LogInformation("CDC Batched Messages: {@Messages}", messages);

        return Task.CompletedTask;
    }

    private class CdcTableModel
    {
        [JsonPropertyName("id")] public int ID { get; set; }
        [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("last_name")] public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}