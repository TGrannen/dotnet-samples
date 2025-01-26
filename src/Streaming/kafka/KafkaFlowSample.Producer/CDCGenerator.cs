namespace KafkaFlowSample.Producer;

public static class CDCGenerator
{
    public static object Generate()
    {
        var payload = new Payload
        {
            before = new Data
            {
                id = 1,
                first_name = "John",
                last_name = "Doe",
                email = "john.doe@example.com",
                updated_at = DateTime.Parse("2024-12-01T10:30:00Z")
            },
            after = new Data
            {
                id = 1,
                first_name = "John",
                last_name = "Smith",
                email = "john.smith@example.com",
                updated_at = DateTime.Parse("2024-12-24T15:00:00Z")
            },
            source = new Source
            {
                version = "2.4.1.Final",
                connector = "mysql",
                name = "dbserver1",
                ts_ms = 1703410800000,
                snapshot = "false",
                db = "testdb",
                table = "customers",
                server_id = 112233,
                file = "mysql-bin.000001",
                pos = 154,
                row = 0,
                thread = 5,
                query = "UPDATE customers SET last_name = 'Smith', email = 'john.smith@example.com' WHERE id = 1"
            },
            op = "u",
            ts_ms = 1703410800000,
            transaction = null
        };
        return new { payload };
    }
}

public class Payload
{
    public Data before { get; set; }
    public Data after { get; set; }
    public Source source { get; set; }
    public string op { get; set; }
    public long ts_ms { get; set; }
    public object transaction { get; set; }
}

public class Data
{
    public int id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string email { get; set; }
    public DateTime updated_at { get; set; }
}

public class Source
{
    public string version { get; set; }
    public string connector { get; set; }
    public string name { get; set; }
    public long ts_ms { get; set; }
    public string snapshot { get; set; }
    public string db { get; set; }
    public string table { get; set; } // Field added based on JSON
    public int server_id { get; set; }
    public string file { get; set; }
    public int pos { get; set; }
    public int row { get; set; }
    public int thread { get; set; }
    public string query { get; set; }
}