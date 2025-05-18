namespace Queue.Domain.Interfaces;

public interface IMessageProcessor
{
    Task ProcessMessageAsync(string message, CancellationToken cancellationToken);
}
