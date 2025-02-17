namespace Services.Kafka.interfaces
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string operation, string message);
    }
}
