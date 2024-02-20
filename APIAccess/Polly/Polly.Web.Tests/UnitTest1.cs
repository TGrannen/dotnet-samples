namespace Polly.Web.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Build your resilience pipeline.
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 4
            })
            .AddTimeout(TimeSpan.FromSeconds(1))
            .Build();

// Retrieve the descriptor.
        ResiliencePipelineDescriptor descriptor = pipeline.GetPipelineDescriptor();

// Check the pipeline's composition with the descriptor.
        Assert.Equal(2, descriptor.Strategies.Count);

// Verify the retry settings.
        var retryOptions = Assert.IsType<RetryStrategyOptions>(descriptor.Strategies[0].Options);
        Assert.Equal(4, retryOptions.MaxRetryAttempts);

// Confirm the timeout settings.
        var timeoutOptions = Assert.IsType<TimeoutStrategyOptions>(descriptor.Strategies[1].Options);
        Assert.Equal(TimeSpan.FromSeconds(1), timeoutOptions.Timeout);
    }
}