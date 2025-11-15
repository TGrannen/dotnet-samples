using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob2(ILogger<ExampleJob2> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Running {Job}", nameof(ExampleJob2));
        await Task.Delay(4000);
        logger.LogInformation("Completed {Job}", nameof(ExampleJob2));
    }
}