namespace Polly.ConsoleApp.Services;

public class TestService
{
    private readonly IOptionsMonitor<TestConfig> _optionsMonitor;
    private readonly ILogger<TestService> _logger;
    private readonly ResiliencePipeline<bool> _resiliencePipeline;

    public TestService(IOptionsMonitor<TestConfig> optionsMonitor, ResiliencePipelineProvider<string> pipelineProvider, ILogger<TestService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;
        _resiliencePipeline = pipelineProvider.GetPipeline<bool>("SQSSendMessage");
    }

    public async Task<bool> Handle(CancellationToken cancellationToken)
    {
        var isSuccess = await _resiliencePipeline.ExecuteAsync(async token =>
        {
            var result = await SendSqsMessage(token);
            return result;
        }, cancellationToken);

        // Need to retry this forever to ensure we don't lose messages (I think?)
        return isSuccess;
    }

    private async Task<bool> SendSqsMessage(CancellationToken cancellationToken)
    {
        var config = _optionsMonitor.CurrentValue;
        _logger.LogInformation("Running with {@Config}", config);
        await Task.Delay(config.Delay, cancellationToken);
        return config.Result;
    }
}