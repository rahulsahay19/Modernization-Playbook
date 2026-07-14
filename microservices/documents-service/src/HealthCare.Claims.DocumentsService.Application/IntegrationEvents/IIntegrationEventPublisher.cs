using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;

namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents
{
    public interface IIntegrationEventPublisher
    {
        Task PublishDocumentRegisteredAsync(DocumentRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
        Task PublishDocumentVerifiedAsync(DocumentVerifiedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
        Task PublishDocumentRejectedAsync(DocumentRejectedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
