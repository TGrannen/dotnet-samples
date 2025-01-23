namespace KafkaFlowSample.Tests;

[Collection(nameof(KafkaContainerFixture))]
public class KafkaExampleTests(KafkaContainerFixture containerFixture)
{
    [Fact]
    public async Task ShouldProduceMessageToKafka()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong1" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        containerFixture.Consumer!.Subscribe("ping");
        var result = containerFixture.Consumer.Consume(TimeSpan.FromSeconds(10));
        result.Message.Value.ShouldBe("pong1");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka1()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong2" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        containerFixture.Consumer!.Subscribe("ping");
        var result = containerFixture.Consumer.Consume(TimeSpan.FromSeconds(10));
        result.Message.Value.ShouldBe("pong2");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka2()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong3" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        containerFixture.Consumer!.Subscribe("ping");
        var result = containerFixture.Consumer.Consume(TimeSpan.FromSeconds(10));
        result.Message.Value.ShouldBe("pong3");
    }

    [Fact]
    public async Task ShouldProduceMessageToKafka3()
    {
        var report = await containerFixture.Producer!.ProduceAsync("ping", new Message<Null, string> { Value = "pong4" });
        report.Status.ShouldBe(PersistenceStatus.Persisted);
        (report.Partition >= 0 && report.Offset >= 0).ShouldBeTrue();

        containerFixture.Consumer!.Subscribe("ping");
        var result = containerFixture.Consumer.Consume(TimeSpan.FromSeconds(10));
        result.Message.Value.ShouldBe("pong4");
    }
}