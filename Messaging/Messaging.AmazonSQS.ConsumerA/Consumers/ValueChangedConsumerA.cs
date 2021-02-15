using MassTransit;
using Messaging.Events.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Messaging.AmazonSQS.ConsumerA.Consumers
{
    public class ValueChangedConsumerA : IConsumer<IValueChanged>
    {
        private static readonly Random Random = new Random();
        private readonly ILogger<ValueChangedConsumerA> _logger;

        public ValueChangedConsumerA(ILogger<ValueChangedConsumerA> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IValueChanged> context)
        {
            _logger.LogDebug("A - Started consuming message: {@Message}", context.Message);
            var delay = Random.Next(2000, 3000);
            await Task.Delay(delay);
            _logger.LogInformation("A - Consumed A message: {@Message}", context.Message);
        }
    }
}