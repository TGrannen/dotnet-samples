using MassTransit;
using Messaging.Configuration.Models;
using Messaging.Events.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Messaging.AmazonSQS.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IAwsSqsConfig _config;
        private readonly ILogger<EventController> _logger;
        private readonly ISendEndpointProvider _send;

        public EventController(ILogger<EventController> logger, IAwsSqsConfig config, ISendEndpointProvider send)
        {
            _logger = logger;
            _config = config;
            _send = send;
        }

        [HttpPost]
        [Route("EmitRecordCreated")]
        public async Task EmitRecordCreated()
        {
            var time = DateTime.Now;
            _logger.LogInformation("Sending IRecordCreated message: {CreatedAt}", time);

            // https://masstransit-project.com/usage/messages.html#messages

            var endpoint = await GetTopicEndpoint();
            await endpoint.Send<IRecordCreated>(
                 new
                 {
                     CreatedAt = time
                 });

            _logger.LogInformation("Message IRecordCreated Sent: {CreatedAt}", time);
        }

        [HttpPost]
        [Route("EmitValueChanged")]
        public async Task EmitValueChanged(string value = "TEST")
        {
            _logger.LogInformation("Sending IValueChanged message: {Value}", value);

            var endpoint = await GetTopicEndpoint();
            await endpoint.Send<IValueChanged>(
                 new
                 {
                     Message = value
                 });

            _logger.LogInformation("Message Sent: {Value}", value);
        }

        private async Task<ISendEndpoint> GetTopicEndpoint()
        {
            var endpoint = await _send.GetSendEndpoint(new Uri($"topic:{_config.TopicName}"));
            return endpoint;
        }
    }
}