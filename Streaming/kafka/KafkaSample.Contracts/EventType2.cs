namespace KafkaSample.Contracts;

public record EventType2
{
    public Guid Id { get; set; }
    public DateTime OccuredAt { get; set; }
    public string StoreNumber { get; set; } = string.Empty;
}