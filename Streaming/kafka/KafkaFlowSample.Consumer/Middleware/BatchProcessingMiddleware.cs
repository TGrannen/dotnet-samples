namespace KafkaFlowSample.Consumer.Middleware;

internal class BatchProcessingMiddleware(ILogger<BatchProcessingMiddleware> logger) : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();

        var messages = batch.Select(ctx => (SampleBatchMessage)ctx.Message.Value).ToList();

        logger.LogInformation("Batched Messages: {@Messages}", messages);

        return Task.CompletedTask;
    }

    internal class SampleBatchMessage
    {
        public string Text { get; set; }
    }
}