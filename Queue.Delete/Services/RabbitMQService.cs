using Queue.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Queue.Delete.Services;

public class RabbitMQService(ILogger<RabbitMQService> logger) : IQueueService
{
    private readonly ILogger<RabbitMQService> _logger = logger;

    public async Task ConsumeAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Waiting for messages on queue {queue}", queueName);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            _logger.LogInformation("Message received: {message}", message);

            await onMessageReceived(message);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(
            queueName,
            autoAck: false,
            consumer,
            cancellationToken);

        while (!cancellationToken.IsCancellationRequested) { await Task.Delay(3000, cancellationToken); }
    }
}
