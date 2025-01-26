using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob2 : IJob
{
    private readonly ILogger<ExampleJob2> _logger;

    public ExampleJob2(ILogger<ExampleJob2> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running {Job}", nameof(ExampleJob2));
        await Task.Delay(4000);
        _logger.LogInformation("Completed {Job}", nameof(ExampleJob2));
    }
}