namespace Queue.Update.Interfaces;

public interface IQueueService
{
    Task ConsumeAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
}