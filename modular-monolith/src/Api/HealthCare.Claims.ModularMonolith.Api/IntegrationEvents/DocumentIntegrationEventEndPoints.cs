using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public static class DocumentIntegrationEventEndpoints
{
    public static IEndpointRouteBuilder MapDocumentIntegrationEventEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/integration-events/documents")
            .WithTags("Cross-service document events");

        group.MapGet("/broker", async (IOptions<IntegrationEventBrokerOptions> options, CancellationToken cancellationToken) =>
        {
            if (string.Equals(options.Value.Transport, "RabbitMq", StringComparison.OrdinalIgnoreCase))
            {
                return await GetRabbitMqBrokerStatus(options.Value, cancellationToken);
            }

            var queueDirectory = BrokerPathResolver.ResolveQueueDirectory(options.Value.QueueDirectory);
            Directory.CreateDirectory(queueDirectory);

            return Results.Ok(new
            {
                transport = "LocalFile",
                broker = "Local file integration-event broker",
                queueDirectory,
                queued = Directory.EnumerateFiles(queueDirectory, "*.json").Count(),
                processed = CountBrokerFiles(queueDirectory, "processed"),
                failed = CountBrokerFiles(queueDirectory, "failed")
            });
        });

        group.MapPost("/registered", (DocumentRegisteredIntegrationEvent integrationEvent, IEventBus eventBus) =>
        {
            eventBus.Publish(new DocumentRegisteredEvent(
                integrationEvent.Id,
                integrationEvent.OccurredOn,
                integrationEvent.ClaimNumber,
                integrationEvent.DocumentType,
                integrationEvent.FileName));

            return Results.Accepted();
        });

        group.MapPost("/verified", (DocumentVerifiedIntegrationEvent integrationEvent, IEventBus eventBus) =>
        {
            eventBus.Publish(new DocumentVerifiedEvent(
                integrationEvent.Id,
                integrationEvent.OccurredOn,
                integrationEvent.ClaimNumber,
                integrationEvent.DocumentType,
                integrationEvent.FileName));

            return Results.Accepted();
        });

        group.MapPost("/rejected", (DocumentRejectedIntegrationEvent integrationEvent, IEventBus eventBus) =>
        {
            eventBus.Publish(new DocumentRejectedEvent(
                integrationEvent.Id,
                integrationEvent.OccurredOn,
                integrationEvent.ClaimNumber,
                integrationEvent.DocumentType,
                integrationEvent.FileName,
                integrationEvent.Reason));

            return Results.Accepted();
        });

        return endpoints;
    }

    private static async Task<IResult> GetRabbitMqBrokerStatus(
        IntegrationEventBrokerOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                options.ExchangeName,
                ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            var queue = await channel.QueueDeclareAsync(
                options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                options.QueueName,
                options.ExchangeName,
                options.RoutingKey,
                cancellationToken: cancellationToken);

            return Results.Ok(new
            {
                transport = "RabbitMq",
                broker = "RabbitMQ integration-event broker",
                host = options.HostName,
                options.Port,
                options.ExchangeName,
                options.QueueName,
                options.RoutingKey,
                queued = queue.MessageCount,
                consumers = queue.ConsumerCount
            });
        }
        catch (Exception exception)
        {
            return Results.Ok(new
            {
                transport = "RabbitMq",
                broker = "RabbitMQ integration-event broker",
                host = options.HostName,
                options.Port,
                options.ExchangeName,
                options.QueueName,
                options.RoutingKey,
                status = "Unavailable",
                error = exception.Message
            });
        }
    }

    private static int CountBrokerFiles(string queueDirectory, string folderName)
    {
        var directory = Path.Combine(queueDirectory, folderName);

        return Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.json").Count()
            : 0;
    }
}

