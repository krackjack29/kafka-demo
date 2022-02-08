using Confluent.Kafka;
using KafkaConsumerDemo.Configuration;
using KafkaConsumerDemo.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace KafkaConsumerDemo
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly KafkaConfigurationOptions kafkaConfiguration;
        private readonly ILogger<KafkaConsumerService> logger;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IOptions<KafkaConfigurationOptions> kafkaConfiguration)
        {
            this.kafkaConfiguration = kafkaConfiguration.Value;
            this.logger = logger; 
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = kafkaConfiguration.GroupId,
                BootstrapServers = kafkaConfiguration.BootStrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
                consumerBuilder.Subscribe(kafkaConfiguration.Topic);
                var cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = consumerBuilder.Consume(cancellationTokenSource.Token);
                        var orderRequest = JsonSerializer.Deserialize<OrderProcessingRequest>(consumer.Message.Value);
                        logger.LogInformation($"Processing order request with id: {orderRequest?.OrderId}");
                    }
                }
                catch (OperationCanceledException ex)
                {
                    consumerBuilder.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while creating consumer: {ex.Message}", ex);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
