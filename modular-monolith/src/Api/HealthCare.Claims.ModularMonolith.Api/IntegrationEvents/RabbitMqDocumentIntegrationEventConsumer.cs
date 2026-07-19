using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed class RabbitMqDocumentIntegrationEventConsumer(
    DocumentIntegrationEventDispatcher dispatcher,
    IOptions<IntegrationEventBrokerOptions> options,
    ILogger<RabbitMqDocumentIntegrationEventConsumer> logger) : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("claimsphere.modular-monolith");
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "RabbitMQ document-event consumer is unavailable; retrying in 5 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
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

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var parentContext = TryGetParentContext(eventArgs.BasicProperties.Headers);
            using var activity = ActivitySource.StartActivity(
                "rabbitmq.consume modular-monolith.documents",
                ActivityKind.Consumer,
                parentContext);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination.name", brokerOptions.QueueName);
            activity?.SetTag("messaging.rabbitmq.routing_key", brokerOptions.RoutingKey);
            activity?.SetTag("messaging.message.id", eventArgs.BasicProperties.MessageId);

            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var brokeredEvent = JsonSerializer.Deserialize<BrokeredIntegrationEvent>(json, JsonOptions)
                    ?? throw new InvalidOperationException("RabbitMQ message did not contain a brokered integration event.");

                activity?.SetTag("event.type", brokeredEvent.EventType);
                dispatcher.Dispatch(brokeredEvent);
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);

                logger.LogInformation(
                    "Consumed RabbitMQ document event {EventType} with message id {MessageId}",
                    brokeredEvent.EventType,
                    brokeredEvent.MessageId);
            }
            catch (Exception exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
                logger.LogWarning(exception, "Could not consume RabbitMQ document event; moving message out of the queue.");
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: brokerOptions.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ document-event consumer is listening on exchange {ExchangeName}, queue {QueueName}",
            brokerOptions.ExchangeName,
            brokerOptions.QueueName);

        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    private static ConnectionFactory CreateConnectionFactory(IntegrationEventBrokerOptions options) =>
        new()
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password
        };

    private static ActivityContext TryGetParentContext(IDictionary<string, object?>? headers)
    {
        var traceParent = GetHeader(headers, "traceparent");
        var traceState = GetHeader(headers, "tracestate");

        return ActivityContext.TryParse(traceParent, traceState, out var context)
            ? context
            : default;
    }

    private static string? GetHeader(IDictionary<string, object?>? headers, string key)
    {
        if (headers is null || !headers.TryGetValue(key, out var value))
        {
            return null;
        }

        return value switch
        {
            null => null,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string text => text,
            _ => value.ToString()
        };
    }
}
