using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Hangfire.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class JobQueueController(ILogger<JobQueueController> logger) : ControllerBase
{
    [HttpPost]
    [Route("QueueExample2")]
    public void QueueExample2()
    {
        logger.LogInformation("Queuing from API action");

        BackgroundJob.Enqueue(() => Log.Information("Log From Background Job!"));

        logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample3")]
    public void QueueExample3(string value)
    {
        logger.LogInformation("Queuing from API action");

        BackgroundJob.Enqueue(() => Log.Information("Log From Background Job with value: {Value}", value));

        logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample4")]
    public void QueueExample4(int delaySeconds)
    {
        var fromSeconds = TimeSpan.FromSeconds(delaySeconds);

        logger.LogInformation("Queuing Delayed Job for {Seconds} seconds. Job should run after {Time}", delaySeconds, DateTimeOffset.Now.Add(fromSeconds));

        BackgroundJob.Schedule(() => Log.Information("Delayed!"), fromSeconds);

        logger.LogInformation("Queued Delayed Job");
    }

    [HttpPost]
    [Route("QueueExample5")]
    public void QueueExample5()
    {
        logger.LogInformation("Queuing Job with Continuation Job");

        var id = BackgroundJob.Schedule(() => Log.Information("Hello, "), TimeSpan.FromSeconds(3));
        BackgroundJob.ContinueJobWith(id, () => Log.Information("World!"));

        logger.LogInformation("Queued Job with Continuation Job");
    }
}