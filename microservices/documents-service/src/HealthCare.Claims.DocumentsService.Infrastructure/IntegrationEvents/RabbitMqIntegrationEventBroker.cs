using System.Text;
using System.Text.Json;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;

public sealed class RabbitMqIntegrationEventBroker(IOptions<IntegrationEventBrokerOptions> options) : IIntegrationEventBroker
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brokeredEvent = new BrokeredIntegrationEvent(
            message.Id,
            message.EventType,
            message.OccurredOn,
            message.Payload);

        var brokerOptions = options.Value;
        var factory = CreateConnectionFactory(brokerOptions);

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            brokerOptions.ExchangeName,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            brokerOptions.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            brokerOptions.QueueName,
            brokerOptions.ExchangeName,
            brokerOptions.RoutingKey,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(brokeredEvent, JsonOptions));
        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = message.Id.ToString(),
            Type = message.EventType,
            Timestamp = new AmqpTimestamp(message.OccurredOn.ToUnixTimeSeconds()),
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            brokerOptions.ExchangeName,
            brokerOptions.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    private static ConnectionFactory CreateConnectionFactory(IntegrationEventBrokerOptions options) =>
        new()
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password
        };
}
