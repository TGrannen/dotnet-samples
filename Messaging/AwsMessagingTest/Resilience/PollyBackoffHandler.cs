using AWS.Messaging.Configuration;
using Polly.Registry;
using Serilog;

namespace AwsMessagingTest.Resilience;

public class PollyBackoffHandler(ResiliencePipelineProvider<string> resiliencePipelineProvider, IOptionsMonitor<TestingConfig> optionsSnapshot) : IBackoffHandler
{
    public async Task<T> BackoffAsync<T>(Func<Task<T>> task, SQSMessagePollerConfiguration configuration, CancellationToken token)
    {
        ResiliencePipeline pipeline = resiliencePipelineProvider.GetPipeline("my-pipeline");
        var delay = optionsSnapshot.CurrentValue.BackoffDelay;
        if (delay.TotalSeconds > 0)
        {
            Log.Information("Message retrieval Delay {Delay}", delay);
            await Task.Delay(delay, token);
        }

        Log.Information("TEST {@Configuration}", configuration);

        // Execute the pipeline
        return await pipeline.ExecuteAsync(async cancellationToken => await task.Invoke(),
            token).ConfigureAwait(false);
    }
}