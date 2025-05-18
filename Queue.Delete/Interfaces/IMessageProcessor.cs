namespace Queue.Delete.Interfaces;

public interface IMessageProcessor
{
    Task ProcessMessageAsync(string message, CancellationToken cancellationToken);
}
