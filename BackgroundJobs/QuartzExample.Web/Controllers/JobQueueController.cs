using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzExample.Web.Jobs;
using System;
using System.Collections.Generic;

namespace QuartzExample.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobQueueController : ControllerBase
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger<JobQueueController> _logger;

        public JobQueueController(IScheduler scheduler, ILogger<JobQueueController> logger)
        {
            _scheduler = scheduler;
            _logger = logger;
        }

        [HttpPost]
        [Route("QueueExample2")]
        public void QueueExample2()
        {
            _logger.LogInformation("Queuing from API action");

            _scheduler.ScheduleImmediateJob<ExampleJob2>();

            _logger.LogInformation("Queued from API action");
        }

        [HttpPost]
        [Route("QueueExample3")]
        public void QueueExample3(string value)
        {
            _logger.LogInformation("Queuing from API action");

            _scheduler.ScheduleImmediateJob<ExampleJob3>(new Dictionary<string, string> { { "value", value } });

            _logger.LogInformation("Queued from API action");
        }

        [HttpPost]
        [Route("QueueExample4")]
        public void QueueExample4(int delaySeconds)
        {
            var fromSeconds = TimeSpan.FromSeconds(delaySeconds);

            _logger.LogInformation("Queuing Delayed Job for {Seconds} seconds. Job should run after {Time}", delaySeconds, DateTimeOffset.Now.Add(fromSeconds));

            _scheduler.ScheduleDelayedJob<ExampleJob2>(fromSeconds);

            _logger.LogInformation("Queued Delayed Job");
        }
    }
}