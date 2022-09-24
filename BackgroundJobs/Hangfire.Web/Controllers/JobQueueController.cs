using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Hangfire.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class JobQueueController : ControllerBase
{
    private readonly ILogger<JobQueueController> _logger;

    public JobQueueController(ILogger<JobQueueController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("QueueExample2")]
    public void QueueExample2()
    {
        _logger.LogInformation("Queuing from API action");

        BackgroundJob.Enqueue(() => Log.Information("Log From Background Job!"));

        _logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample3")]
    public void QueueExample3(string value)
    {
        _logger.LogInformation("Queuing from API action");

        BackgroundJob.Enqueue(() => Log.Information("Log From Background Job with value: {Value}", value));

        _logger.LogInformation("Queued from API action");
    }

    [HttpPost]
    [Route("QueueExample4")]
    public void QueueExample4(int delaySeconds)
    {
        var fromSeconds = TimeSpan.FromSeconds(delaySeconds);

        _logger.LogInformation("Queuing Delayed Job for {Seconds} seconds. Job should run after {Time}", delaySeconds, DateTimeOffset.Now.Add(fromSeconds));

        BackgroundJob.Schedule(() => Log.Information("Delayed!"), fromSeconds);

        _logger.LogInformation("Queued Delayed Job");
    }

    [HttpPost]
    [Route("QueueExample5")]
    public void QueueExample5()
    {
        _logger.LogInformation("Queuing Job with Continuation Job");

        var id = BackgroundJob.Schedule(() => Log.Information("Hello, "), TimeSpan.FromSeconds(3));
        BackgroundJob.ContinueJobWith(id, () => Log.Information("World!"));

        _logger.LogInformation("Queued Job with Continuation Job");
    }
}