using Queue.Domain.Interfaces;

namespace Queue.Insert;

public class Worker(
    IQueueService queue,
    IMessageProcessor messageProcessor) : BackgroundService
{
    private readonly IQueueService _queueService = queue;
    private readonly IMessageProcessor _messageProcessor = messageProcessor;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _queueService.ConsumeAsync(
            "Contact.Queue.Insert",
            message => _messageProcessor.ProcessMessageAsync(message, cancellationToken),
            cancellationToken);
    }
}
