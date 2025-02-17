using Confluent.Kafka;
using Data.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Kafka.interfaces;

namespace Services.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly string _kafkaBootstrapServers;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _kafkaBootstrapServers = configuration.GetKafkaSetting("BootstrapServers");

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaBootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task ProduceAsync(string operation, string message)
        {
            try
            {
                var topic = "permissions-topic";
                var result = await _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = $"{{\"Operation\":\"{operation}\", \"Message\":\"{message}\"}}"
                });
                _logger.LogInformation($"Produced message to {result.TopicPartitionOffset}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error producing message: {ex.Message}");
            }
        }
    }
}