namespace KafkaSample.Contracts;

public record EventType1
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}