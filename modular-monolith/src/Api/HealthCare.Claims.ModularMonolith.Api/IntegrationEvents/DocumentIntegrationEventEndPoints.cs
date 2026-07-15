using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents
{
    public static class DocumentIntegrationEventEndpoints
    {
        public static IEndpointRouteBuilder MapDocumentIntegrationEventEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/integration-events/documents")
                .WithTags("Cross-service document events");

            group.MapGet("/broker", (IOptions<IntegrationEventBrokerOptions> options) =>{
                var queueDirectory = BrokerPathResolver.ResolveQueueDirectory(options.Value.QueueDirectory);
                Directory.CreateDirectory(queueDirectory);

                return Results.Ok(new
                {
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

        private static int CountBrokerFiles(string queueDirectory, string folderName)
        {
            var directory = Path.Combine(queueDirectory, folderName);
            return Directory.Exists(directory)
                ? Directory.EnumerateFiles(directory, "*.json").Count()
                : 0;
        }
    }
}
