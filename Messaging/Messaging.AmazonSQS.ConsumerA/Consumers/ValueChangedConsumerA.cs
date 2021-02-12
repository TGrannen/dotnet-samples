using MassTransit;
using Messaging.Events.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Messaging.AmazonSQS.ConsumerA.Consumers
{
    public class ValueChangedConsumerA : IConsumer<IValueChanged>
    {
        private readonly ILogger<ValueChangedConsumerA> _logger;

        public ValueChangedConsumerA(ILogger<ValueChangedConsumerA> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IValueChanged> context)
        {
            _logger.LogInformation("Consumed A message: {@Message}", context.Message);
            return Task.CompletedTask;
        }
    }
}