using Confluent.Kafka;
using KafkaProducerDemo.Configuration;
using KafkaProducerDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace KafkaProducerDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly KafkaConfigurationOptions kafkaConfiguration;
        private readonly ILogger<ProducerController> logger;

        public ProducerController(IOptions<KafkaConfigurationOptions> kafkaConfigurationOptions, 
              ILogger<ProducerController> logger)
        {
            this.kafkaConfiguration = kafkaConfigurationOptions.Value;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrders([FromBody] OrderRequest request)
        {
            var message = JsonSerializer.Serialize(request);
            return Ok(await SendOrderRequest(message));
        }

        private async Task<bool> SendOrderRequest(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaConfiguration.BootStrapServers,
                ClientId = Dns.GetHostName(),
            };
            try
            {
                using var producer = new ProducerBuilder<Null, string>(config).Build();
                // C# 8 has a simplified using statement without using braces
                var result = await producer.ProduceAsync(kafkaConfiguration.Topic, new Message<Null, string>
                {
                    Value = message
                });
                logger.LogInformation($"Delivery timestamp: {result.Timestamp.UtcDateTime}");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception occured while sending message : {ex.Message}");
                return false;
            }
        }
    }
}
