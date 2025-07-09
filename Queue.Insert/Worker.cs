using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace Queue.Insert;

public class Worker(
    IRepositoryBase repository,
    ILogger<Worker> logger) : BackgroundService
{
    private readonly IRepositoryBase _repository = repository;
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) => await Init(cancellationToken);

    private async Task Init(CancellationToken cancellationToken, int tries = 1)
    {
        try
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq" };
            using var conn = await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Conectado ao RabbitMQ");

            await Processar(channel, "Contact.Queue.Insert", cancellationToken);

            while (!cancellationToken.IsCancellationRequested) { }
        }
        catch (BrokerUnreachableException)
        {
            _logger.LogInformation("Falha ao conectar ao RabbitMQ - Tentativa {t}", tries);

            if (tries >= 5)
                throw new Exception("Erro ao conectar no rabbitMQ, tentativas excedidas");

            Thread.Sleep(10_000);

            await Init(cancellationToken, tries + 1);
        }
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

            var entity = JsonConvert.DeserializeObject<Contact>(message);

            _logger.LogInformation("{m}", message);

            await _repository.Insert(entity, cancellationToken);

            await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);
    }
}
