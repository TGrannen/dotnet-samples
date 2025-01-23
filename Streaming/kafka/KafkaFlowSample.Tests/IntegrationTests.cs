using System.Net.Http.Json;

namespace KafkaFlowSample.Tests;

[Collection(nameof(KafkaWebApplicationFixture))]
public class IntegrationTests(KafkaWebApplicationFixture factory)
{
    [Fact]
    public async Task ProduceEndpoint_Should()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/produce", new { });

        response.EnsureSuccessStatusCode();
        var result = factory.ConsumeFromTopic("sample-topic");

        await VerifyJson(result.Message.Value);
    }
}