namespace Queue.Insert.Interfaces;

public interface IMessageProcessor
{
    Task ProcessMessageAsync(string message, CancellationToken cancellationToken);
}
