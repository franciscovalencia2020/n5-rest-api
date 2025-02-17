using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.Kafka
{
    public class KafkaConsumerHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly string _topic = "permissions-topic";
        private readonly string _groupId = "permissions-consumer-group";
        private readonly string _bootstrapServers = "localhost:9092";
        private IConsumer<Ignore, string> _consumer;
        private CancellationTokenSource _cts;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError($"Kafka Error: {e.Reason}"))
                .Build();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Run(() => ConsumeMessages(), _cts.Token);

            _logger.LogInformation("Kafka consumer started.");
            return Task.CompletedTask;
        }

        private async Task ConsumeMessages()
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(_cts.Token);
                    _logger.LogInformation($"Consumed message: {consumeResult.Value}");
                    await File.AppendAllTextAsync("kafka_logs.txt", $"{DateTime.Now}: {consumeResult.Value}\n");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer shutting down.");
            }
            finally
            {
                _consumer.Close();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumer?.Dispose();
            _cts?.Dispose();
        }
    }
}
