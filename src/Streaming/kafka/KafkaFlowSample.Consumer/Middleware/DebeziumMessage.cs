using System.Text.Json.Serialization;

namespace KafkaFlowSample.Consumer.Middleware;

public class DebeziumMessage<T>
{
    [JsonPropertyName("payload")] public PayloadModel Payload { get; set; }

    public class PayloadModel
    {
        [JsonPropertyName("before")] public T Before { get; set; }
        [JsonPropertyName("after")] public T After { get; set; }
        [JsonPropertyName("source")] public SourceModel Source { get; set; }

        public class SourceModel
        {
            [JsonPropertyName("version")] public string version { get; set; }
            [JsonPropertyName("connector")] public string connector { get; set; }
            [JsonPropertyName("name")] public string name { get; set; }
            [JsonPropertyName("ts_ms")] public long ts_ms { get; set; }
            [JsonPropertyName("snapshot")] public string snapshot { get; set; }
            [JsonPropertyName("db")] public string db { get; set; }
            [JsonPropertyName("table")] public string table { get; set; }
            [JsonPropertyName("server_id")] public int server_id { get; set; }
            [JsonPropertyName("file")] public string file { get; set; }
            [JsonPropertyName("pos")] public int pos { get; set; }
            [JsonPropertyName("row")] public int row { get; set; }
            [JsonPropertyName("thread")] public int thread { get; set; }
            [JsonPropertyName("query")] public string query { get; set; }
        }
    }
}