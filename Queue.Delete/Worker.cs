using Queue.Domain.Interfaces;

namespace Queue.Delete;

public class Worker(
    IQueueService queue,
    IMessageProcessor message) : BackgroundService
{
    private readonly IQueueService _queueService = queue;
    private readonly IMessageProcessor _messageProcessor = message;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _queueService.ConsumeAsync(
            "Contact.Queue.Delete",
            message => _messageProcessor.ProcessMessageAsync(message, cancellationToken),
            cancellationToken);
    }
}
