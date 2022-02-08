namespace KafkaConsumerDemo.Configuration
{
    public class KafkaConfigurationOptions
    {
        public const string ConfigurationRootName = "Kafka";
        public string BootStrapServers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty ;
        public string GroupId { get; set; } = string.Empty ;
    }
}
