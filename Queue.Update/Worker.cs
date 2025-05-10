using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Queue.Update;

public class Worker(
    IRepositoryBase repository,
    ILogger<Worker> logger) : BackgroundService
{
    private readonly IRepositoryBase _repository = repository;
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var conn = await factory.CreateConnectionAsync(cancellationToken);
        using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

        await Processar(channel, "Contact.Queue.Update", cancellationToken);

        while (!cancellationToken.IsCancellationRequested) { }
    }

    private async Task Processar(IChannel channel, string queue, CancellationToken cancellationToken)
    {
        await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        _logger.LogInformation("Waiting messages...");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Message received: {message}", message);

            var entity = JsonConvert.DeserializeObject<Contact>(message);

            await _repository.Update(entity, cancellationToken);

            await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);
    }
}
