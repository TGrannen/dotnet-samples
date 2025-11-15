using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob1(ILogger<ExampleJob1> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Running {Job}", nameof(ExampleJob1));
        await Task.Delay(1000);
        logger.LogInformation("Completed {Job}", nameof(ExampleJob1));
    }
}