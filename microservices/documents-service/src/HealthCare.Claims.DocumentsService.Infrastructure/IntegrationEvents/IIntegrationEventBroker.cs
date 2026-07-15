using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public interface IIntegrationEventBroker
    {
        Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken);
    }
}
