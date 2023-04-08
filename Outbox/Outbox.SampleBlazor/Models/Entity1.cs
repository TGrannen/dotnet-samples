using Amazon.DynamoDBv2.DataModel;

namespace Outbox.SampleBlazor.Models;

[DynamoDBTable("Test-Entity1")]
public class Entity1
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}