using Polly.CircuitBreaker;
using Polly.Retry;
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
                    Name = "Retry Forever",
                    ShouldHandle = args => ValueTask.FromResult(args switch
                    {
                        { Outcome.Result: true } => false,
                        _ => true
                    }),
                    MaxRetryAttempts = int.MaxValue,
                    Delay = config.CircuitBreakDelay,
                    BackoffType = DelayBackoffType.Constant,
                    OnRetry = arguments =>
                    {
                        Log.Information("Retry Forever - On Retry {@Args}",
                            new { arguments.Outcome.Result, arguments.Outcome.Exception?.Message, arguments.RetryDelay, arguments.AttemptNumber });
                        return default;
                    }
                })
                .AddRetry(new RetryStrategyOptions<bool>
                {
                    ShouldHandle = args => ValueTask.FromResult(args switch
                    {
                        { Outcome.Result: true } => false,
                        { Outcome.Exception: BrokenCircuitException } => false,
                        _ => true
                    }),
                    MaxRetryAttempts = config.RetryAttempts,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = arguments =>
                    {
                        Log.Information("On Retry {@Args}",
                            new { arguments.Outcome.Result, arguments.Outcome.Exception?.Message, arguments.RetryDelay, arguments.AttemptNumber });
                        return default;
                    }
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions<bool>
                {
                    Name = "SQS Circuit Breaker",
                    SamplingDuration = TimeSpan.FromSeconds(60),
                    MinimumThroughput = config.RetryAttempts + 1,
                    BreakDuration = config.CircuitBreakDelay,
                    ShouldHandle = args => ValueTask.FromResult(args switch
                    {
                        { Outcome.Result: true } => false,
                        _ => true
                    }),
                    OnOpened = arguments =>
                    {
                        Log.Information("Circuit Breaker - OnOpened {@Args}", new { arguments.Outcome.Result, arguments.BreakDuration });
                        return default;
                    },
                });
        });
    }

    public class PipelineConfig
    {
        public int RetryAttempts { get; set; }
        public TimeSpan CircuitBreakDelay { get; set; }
    }
}