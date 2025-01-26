using Amazon.DynamoDBv2.DataModel;

namespace Outbox.SampleBlazor.Models;

[DynamoDBTable("Test-Entity2")]
public class Entity2
{
    public string Id { get; set; }
    public DateTime LastUpdated { get; set; }
}