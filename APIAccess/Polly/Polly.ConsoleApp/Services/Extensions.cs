using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Serilog;

namespace Polly.ConsoleApp.Services;

public static class Extensions
{
    public static void AddSQSSendMessagePipeline(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("PipelineConfig").Get<PipelineConfig>();
        services.AddResiliencePipeline<string, bool>("SQSSendMessage", builder =>
        {
            builder
                .AddRetry(new RetryStrategyOptions<bool>
                {
                    ShouldHandle = args => ValueTask.FromResult(args switch
                    {
                        { Outcome.Result: true } => false,
                        { Outcome.Exception: BrokenCircuitException } => false,
                        _ => true
                    }),
                    MaxRetryAttempts = config.RetryAttempts,
                    MaxDelay = config.Delay,
                    Delay = config.MaxDelay,
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = arguments =>
                    {
                        Log.Information("Retrying SQS message sending {@Args}",
                            new { arguments.Outcome.Result, arguments.Outcome.Exception?.Message, arguments.RetryDelay, arguments.AttemptNumber });
                        return default;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = config.Timeout,
                    OnTimeout = arguments =>
                    {
                        Log.Information("On Timeout {@Args}", new { arguments.Timeout });
                        return default;
                    }
                });
        });
    }

    public class PipelineConfig
    {
        public int RetryAttempts { get; init; }
        public TimeSpan Timeout { get; init; }= TimeSpan.FromSeconds(10);
        public TimeSpan Delay { get; init; }= TimeSpan.FromSeconds(2);
        public TimeSpan MaxDelay { get; init; }= TimeSpan.FromMinutes(2);
    }
}