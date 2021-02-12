using MassTransit.AmazonSqsTransport;
using System;

namespace Messaging.AmazonSQS.Extensions
{
    public static class Extensions
    {
        public static void AddAwsTopicAndQueueEndpoint(this IAmazonSqsBusFactoryConfigurator cfg,
            string topic,
            string queue,
            Action<IAmazonSqsReceiveEndpointConfigurator> action)
        {
            cfg.ReceiveEndpoint(queue, e =>
            {
                // disable the default topic binding
                e.ConfigureConsumeTopology = false;
                e.Subscribe(topic);
                action(e);
            });
        }
    }
}