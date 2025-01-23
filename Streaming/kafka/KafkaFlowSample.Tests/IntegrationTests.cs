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
        factory.Consumer!.Subscribe("sample-topic");
        var result = factory.Consumer.Consume(TimeSpan.FromSeconds(10));

        await VerifyJson(result.Message.Value);
    }
}