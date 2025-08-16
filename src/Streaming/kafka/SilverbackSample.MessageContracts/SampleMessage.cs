namespace SilverbackSample.MessageContracts;

public class SampleMessage : IIntegrationEvent
{
    [KafkaKeyMember] public int Number { get; set; }
    public DateTimeOffset UtcNow { get; set; }
}