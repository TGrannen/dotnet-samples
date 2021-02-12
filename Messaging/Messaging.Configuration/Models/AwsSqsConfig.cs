namespace Messaging.Configuration.Models
{
    public class AwsSqsConfig : IAwsSqsConfig
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string HostName { get; set; }
        public string TopicName { get; set; }
    }
}