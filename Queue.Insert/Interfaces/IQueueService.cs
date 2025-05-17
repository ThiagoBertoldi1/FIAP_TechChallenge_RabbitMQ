namespace Queue.Insert.Interfaces;

public interface IQueueService
{
    Task ConsumeAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
}
