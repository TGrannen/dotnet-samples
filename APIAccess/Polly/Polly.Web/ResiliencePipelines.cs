using Polly.CircuitBreaker;
using Polly.Retry;

namespace Polly.Web;

public static class ResiliencePipelines
{
    public const string GithubRetry = "Github-Retry";
    public const string Test = "Tests";

    public static IServiceCollection AddResilience(this IServiceCollection services)
    {
        AddGithubRetry(services);
        AddTestWith(services);
        return services;
    }

    private static void AddGithubRetry(IServiceCollection services)
    {
        services.AddResiliencePipeline<string, HttpResponseMessage>(GithubRetry, static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = args => ValueTask.FromResult(args switch
                {
                    { Outcome: { Exception: HttpRequestException } } => true,
                    { Outcome: { Exception: InvalidOperationException } } => true,
                    { Outcome: { Result: { IsSuccessStatusCode: false } } } => true,
                    _ => false
                }),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = static args =>
                {
                    Log.Warning("Retry Number: {RetryCount}  Waiting: {Duration}, due to: {Message}", args.AttemptNumber, args.RetryDelay,
                        args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                }
            });
        });
    }

    private static void AddTestWith(IServiceCollection services)
    {
        services.AddResiliencePipeline<string, bool>(Test, static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new RetryStrategyOptions<bool>
            {
                ShouldHandle = args => ValueTask.FromResult(args switch
                {
                    { Outcome: { Exception: BrokenCircuitException } } => false,
                    { Outcome: { Exception: not null } } => true,
                    { Outcome: { Result: false } } => true,
                    _ => false
                }),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                // OnRetry = static args =>
                // {
                //     Log.Warning("Retry Number: {RetryCount}  Waiting: {Duration}, due to: {Message}", args.AttemptNumber, args.RetryDelay,
                //         args.Outcome.Exception?.Message);
                //     return ValueTask.CompletedTask;
                // }
            });

            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<bool>
            {
                // Customize and configure the circuit breaker logic.
                SamplingDuration = TimeSpan.FromSeconds(45),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = static args => ValueTask.FromResult(args.Outcome.Result == false),
                // OnOpened = arguments =>
                // {
                //     Log.Error("Opened Args {@Arg}", new
                //     {
                //         arguments.BreakDuration,
                //         arguments.Outcome.Exception,
                //         arguments.Outcome.Result,
                //         arguments.IsManual,
                //     });
                //     return ValueTask.CompletedTask;
                // },
                // OnHalfOpened = arguments =>
                // {
                //     Log.Warning("Half open");
                //     return ValueTask.CompletedTask;
                // },
                // OnClosed = arguments =>
                // {
                //     Log.Warning("Closed Args {@Arg}", new
                //     {
                //         arguments.Outcome.Exception,
                //         arguments.Outcome.Result,
                //         arguments.IsManual,
                //     });
                //     return ValueTask.CompletedTask;
                // }
            });

            builder.AddTimeout(TimeSpan.FromSeconds(5));
        });
    }
}