using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents
{
    public static class DocumentIntegrationEventEndpoints
    {
        public static IEndpointRouteBuilder MapDocumentIntegrationEventEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/integration-events/documents")
                .WithTags("Cross-service document events");

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
    }
}
