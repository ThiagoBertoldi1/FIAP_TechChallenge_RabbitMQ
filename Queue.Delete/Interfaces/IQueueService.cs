namespace Queue.Delete.Interfaces;

public interface IQueueService
{
    Task ConsumeAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
}
