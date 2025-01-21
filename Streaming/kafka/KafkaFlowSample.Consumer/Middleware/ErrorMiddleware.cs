using Polly;
using Polly.Retry;

namespace KafkaFlowSample.Consumer.Middleware;

public class ErrorMiddleware : IMessageMiddleware
{
    private readonly ILogger<ErrorMiddleware> _logger;
    private readonly ResiliencePipeline _pipeline;

    public ErrorMiddleware(ILogger<ErrorMiddleware> logger)
    {
        _logger = logger;
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                MaxDelay = TimeSpan.FromSeconds(15),
                MaxRetryAttempts = 1000,
                OnRetry = arguments =>
                {
                    logger.LogWarning("Retrying {AttemptNumber} after {Delay} delay", arguments.AttemptNumber, arguments.RetryDelay);
                    return ValueTask.CompletedTask;
                }
            }) // Add retry using the default options
            .AddTimeout(TimeSpan.FromSeconds(10)) // Add 10 seconds timeout
            .Build();
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await _pipeline.ExecuteAsync(async token => { await next(context); }, context.ConsumerContext.WorkerStopped).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            context.ConsumerContext.AutoMessageCompletion = false;
            _logger.LogError("Message processing has been cancelled");
        }
        catch (Exception e)
        {
            context.ConsumerContext.AutoMessageCompletion = false;
            _logger.LogError("Middleware got exception");
            throw;
        }
    }
}