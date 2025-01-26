namespace KafkaFlowSample.Consumer.Configuration;

public class ConsumerConfigurationTest
{
    public string[] Topics { get; set; } = [];
    public string ConsumerGroupId { get; set; } = string.Empty;
    public int BufferSize { get; set; } = 100;
    public int WorkerCount { get; set; } = 1;
    public TimeSpan WorkerStoppedTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public BatchingConfig? Batching { get; set; }
    public string? StaticTypeName { get; set; }
    public string? CdcTypeName { get; set; }

    public class BatchingConfig
    {
        public int BatchSize { get; set; } = 100;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}