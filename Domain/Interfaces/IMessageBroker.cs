namespace Domain.Interfaces
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
        Task PublishAsync(string queueName, string message);
        Task PublishAsync(string queueName, byte[] body);
    }
}