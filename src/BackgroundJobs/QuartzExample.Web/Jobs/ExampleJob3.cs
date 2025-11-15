using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob3(ILogger<ExampleJob3> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;

        string value = dataMap.GetString("value");

        logger.LogInformation("Running {Job} with Value: {Value}", nameof(ExampleJob3), value);

        await Task.Delay(3000);

        logger.LogInformation("Completed {Job}", nameof(ExampleJob3));
    }
}