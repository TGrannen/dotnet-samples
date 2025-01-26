using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob1 : IJob
{
    private readonly ILogger<ExampleJob1> _logger;

    public ExampleJob1(ILogger<ExampleJob1> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running {Job}", nameof(ExampleJob1));
        await Task.Delay(1000);
        _logger.LogInformation("Completed {Job}", nameof(ExampleJob1));
    }
}