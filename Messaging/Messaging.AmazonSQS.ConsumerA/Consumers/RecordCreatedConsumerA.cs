using MassTransit;
using Messaging.Events.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Messaging.AmazonSQS.ConsumerA.Consumers
{
    public class RecordCreatedConsumerA : IConsumer<IRecordCreated>
    {
        private readonly ILogger<RecordCreatedConsumerA> _logger;

        public RecordCreatedConsumerA(ILogger<RecordCreatedConsumerA> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IRecordCreated> context)
        {
            _logger.LogInformation("Consumed A message: {@Message}", context.Message);
            return Task.CompletedTask;
        }
    }
}