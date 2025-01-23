namespace KafkaFlowSample.Tests;

[Collection(nameof(KafkaWebApplicationFixture))]
public class KafkaExampleTests(KafkaWebApplicationFixture containerFixture)
{
    [Fact]
    public async Task ShouldProduceMessageToKafka()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong1" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        var result = containerFixture.ConsumeFromTopic("ping");
        result.Message.Value.ShouldBe("pong1");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka1()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong2" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        var result = containerFixture.ConsumeFromTopic("ping");
        result.Message.Value.ShouldBe("pong2");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka2()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong3" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        var result = containerFixture.ConsumeFromTopic("ping");
        result.Message.Value.ShouldBe("pong3");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka3()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong4" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        var result = containerFixture.ConsumeFromTopic("ping");
        result.Message.Value.ShouldBe("pong4");
    }
}