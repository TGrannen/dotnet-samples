namespace KafkaFlowSample.Consumer.Middleware;

internal class BatchCompletionProcessingMiddleware : IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        await next(context).ConfigureAwait(false);

        foreach (var messageContext in context.GetMessagesBatch())
        {
            messageContext.ConsumerContext.Complete();
        }
    }
}