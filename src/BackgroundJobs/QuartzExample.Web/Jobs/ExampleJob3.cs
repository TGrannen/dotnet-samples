using Quartz;

namespace QuartzExample.Web.Jobs;

[DisallowConcurrentExecution]
public class ExampleJob3 : IJob
{
    private readonly ILogger<ExampleJob3> _logger;

    public ExampleJob3(ILogger<ExampleJob3> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;

        string value = dataMap.GetString("value");

        _logger.LogInformation("Running {Job} with Value: {Value}", nameof(ExampleJob3), value);

        await Task.Delay(3000);

        _logger.LogInformation("Completed {Job}", nameof(ExampleJob3));
    }
}