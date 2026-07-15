using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints
{
    public static class OutboxEndpoints
    {
        public static IEndpointRouteBuilder MapOutboxEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/outbox", async (
                IOutboxStore outboxStore,
                IOptions<IntegrationEventBrokerOptions> Options,
                CancellationToken cancelllationToken) =>
            {
                var messages = await outboxStore.SnapshotAsync(cancelllationToken);
                return Results.Ok(new
                {
                    broker = "Local file integration-event broker",
                    queueDirectory = BrokerPathResolver.ResolveQueueDirectory(Options.Value.QueueDirectory),
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
}
