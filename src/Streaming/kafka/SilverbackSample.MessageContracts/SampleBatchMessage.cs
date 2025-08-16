namespace SilverbackSample.MessageContracts;

public class SampleBatchMessage : IIntegrationEvent
{
    [KafkaKeyMember] public int Number { get; set; }
    public DateTimeOffset UtcNow { get; set; }
}