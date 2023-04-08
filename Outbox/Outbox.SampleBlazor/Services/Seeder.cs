using System.Diagnostics;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Outbox.SampleBlazor.Services;

public interface ISeeder
{
    Task Initialize();
}

public class Seeder : ISeeder
{
    private readonly IAmazonDynamoDB _client;
    private readonly ILogger<Seeder> _logger;

    public Seeder(IConfiguration configuration, ILogger<Seeder> logger)
    {
        var serviceUrl = configuration.GetValue<string>("AWS:Dynamo:LocalUrl");
        var amazonDynamoDb = new AmazonDynamoDBClient("123", "123",
            new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1,
                UseHttp = true,
                ServiceURL = serviceUrl
            });

        _client = amazonDynamoDb;
        _logger = logger;
    }

    public async Task Initialize()
    {
        _logger.LogInformation("Creating Dynamo DB tables");
        await CreateTable("Test-Entity1", "Id", ScalarAttributeType.S, KeyType.HASH);
        await CreateTable("Test-Entity2", "Id", ScalarAttributeType.S, KeyType.HASH);
        await CreateTable("Outbox-Shared", "Key", ScalarAttributeType.S, KeyType.HASH);
    }

    private async Task CreateTable(string tableName, string keyName, ScalarAttributeType attributeType, KeyType keyType)
    {
        var tableCreateRequest = new CreateTableRequest
        {
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new() { AttributeName = keyName, AttributeType = attributeType }
            },
            TableName = tableName,
            KeySchema = new List<KeySchemaElement>
            {
                new() { AttributeName = keyName, KeyType = keyType }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 6
            }
        };
        var response = await _client.CreateTableAsync(tableCreateRequest);
        Debug.Assert(response.HttpStatusCode == HttpStatusCode.OK,
            $"Failed to create Dynamo DB table {tableName} needed for test execution");
        _logger.LogInformation("{Table} was created", tableName);
    }
}