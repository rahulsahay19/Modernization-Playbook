using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class HttpIntegrationEventPublisher(
    HttpClient httpClient,
    IOptions<IntegrationEventRelayOptions> options,
    ILogger<HttpIntegrationEventPublisher> logger) : IIntegrationEventPublisher
    {
        public Task PublishDocumentRegisteredAsync(
            DocumentRegisteredIntegrationEvent integrationEvent,
            CancellationToken cancellationToken) =>
            PublishAsync("registered", integrationEvent, cancellationToken);

        public Task PublishDocumentVerifiedAsync(
            DocumentVerifiedIntegrationEvent integrationEvent,
            CancellationToken cancellationToken) =>
            PublishAsync("verified", integrationEvent, cancellationToken);

        public Task PublishDocumentRejectedAsync(
            DocumentRejectedIntegrationEvent integrationEvent,
            CancellationToken cancellationToken) =>
            PublishAsync("rejected", integrationEvent, cancellationToken);

        private async Task PublishAsync<TEvent>(
            string eventRoute,
            TEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            var endpoint = new Uri(new Uri(options.Value.ModularMonolithBaseUrl), $"/api/integration-events/documents/{eventRoute}");

            try
            {
                using var response = await httpClient.PostAsJsonAsync(endpoint, integrationEvent, cancellationToken);
                response.EnsureSuccessStatusCode();
                logger.LogInformation("Published document integration event to {Endpoint}", endpoint);
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning(
                    exception,
                    "Could not publish document integration event to {Endpoint}. The document command still completed locally.",
                    endpoint);
            }
        }
    }
}
