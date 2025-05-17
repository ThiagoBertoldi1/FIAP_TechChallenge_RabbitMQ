using Queue.Update.Interfaces;

namespace Queue.Update;

public class Worker(
    IQueueService queueService,
    IMessageProcessor messageProcessor) : BackgroundService
{
    private readonly IQueueService _queueService = queueService;
    private readonly IMessageProcessor _messageProcessor = messageProcessor;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _queueService.ConsumeAsync(
            "Contact.Queue.Update",
            message => _messageProcessor.ProcessMessageAsync(message, cancellationToken),
            cancellationToken);
    }
}
