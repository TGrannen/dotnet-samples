namespace Messaging.Configuration.Models;

public interface IAwsSqsConfig
{
    string AccessKey { get; set; }
    string SecretKey { get; set; }
    string HostName { get; set; }
    string TopicName { get; set; }
}