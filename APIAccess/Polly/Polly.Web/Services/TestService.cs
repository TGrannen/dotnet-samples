using Microsoft.Extensions.Options;

namespace Polly.Web.Services;

public class TestOptions
{
    public TimeSpan DelayTime { get; set; }
    public bool ThrowException { get; set; }
    public bool ReturnFalse { get; set; }
}

public interface ITestService
{
    Task<bool> RunTest();
}

public class TestService : ITestService
{
    private readonly IOptionsMonitor<TestOptions> _optionsMonitor;
    private readonly ILogger<TestService> _logger;
    private readonly ResiliencePipeline<bool> _pipeline;

    public TestService(ResiliencePipelineProvider<string> pipelineProvider, IOptionsMonitor<TestOptions> optionsMonitor,
        ILogger<TestService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;
        _pipeline = pipelineProvider.GetPipeline<bool>(ResiliencePipelines.Test);
    }

    public async Task<bool> RunTest()
    {
        _logger.LogInformation("Running Test");

        var result = await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var config = _optionsMonitor.CurrentValue;
            await Task.Delay(config.DelayTime, cancellationToken);

            if (config.ReturnFalse)
            {
                return false;
            }

            if (config.ThrowException)
            {
                throw new Exception("This is a test exception");
            }

            return true;
        });

        return result;
    }
}