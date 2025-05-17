namespace Queue.Update.Interfaces;

public interface IMessageProcessor
{
    Task ProcessMessageAsync(string message, CancellationToken cancellationToken);
}