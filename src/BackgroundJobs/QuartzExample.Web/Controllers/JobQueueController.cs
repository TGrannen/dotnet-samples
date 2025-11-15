using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzExample.Web.Extensions;
using QuartzExample.Web.Jobs;

namespace QuartzExample.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class JobQueueController(IScheduler scheduler, ILogger<JobQueueController> logger) : ControllerBase
{
    [HttpPost]
    [Route("QueueExample2")]
    public void QueueExample2()
    {
        logger.LogInformation("Queuing from API action");

        scheduler.ScheduleImmediateJob<ExampleJob2>();

        logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample3")]
    public void QueueExample3(string value)
    {
        logger.LogInformation("Queuing from API action");

        scheduler.ScheduleImmediateJob<ExampleJob3>(new Dictionary<string, string> { { "value", value } });

        logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample4")]
    public void QueueExample4(int delaySeconds)
    {
        var fromSeconds = TimeSpan.FromSeconds(delaySeconds);

        logger.LogInformation("Queuing Delayed Job for {Seconds} seconds. Job should run after {Time}", delaySeconds, DateTimeOffset.Now.Add(fromSeconds));

        scheduler.ScheduleDelayedJob<ExampleJob2>(fromSeconds);

        logger.LogInformation("Queued Delayed Job");
    }
}