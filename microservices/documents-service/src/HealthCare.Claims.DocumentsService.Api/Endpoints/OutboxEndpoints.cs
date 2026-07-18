using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class OutboxEndpoints
{
    public static IEndpointRouteBuilder MapOutboxEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/outbox", async (
            IOutboxStore outboxStore,
            IOptions<IntegrationEventBrokerOptions> options,
            CancellationToken cancellationToken) =>
        {
            var messages = await outboxStore.SnapshotAsync(cancellationToken);

            return Results.Ok(new
            {
                transport = options.Value.Transport,
                broker = string.Equals(options.Value.Transport, "RabbitMq", StringComparison.OrdinalIgnoreCase)
                    ? "RabbitMQ integration-event broker"
                    : "Local file integration-event broker",
                rabbitMq = new
                {
                    options.Value.HostName,
                    options.Value.Port,
                    options.Value.ExchangeName,
                    options.Value.QueueName,
                    options.Value.RoutingKey
                },
                queueDirectory = string.Equals(options.Value.Transport, "LocalFile", StringComparison.OrdinalIgnoreCase)
                    ? BrokerPathResolver.ResolveQueueDirectory(options.Value.QueueDirectory)
                    : null,
                messages = messages.Select(message => new
                {
                    message.Id,
                    message.EventType,
                    message.OccurredOn,
                    Status = message.Status.ToString(),
                    message.Attempts,
                    message.LastError,
                    message.PublishedOn
                })
            });
        }).WithTags("Integration events");

        return endpoints;
    }
}
