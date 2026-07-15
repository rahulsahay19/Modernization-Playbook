namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class IntegrationEventBrokerOptions
    {
        public string QueueDirectory { get; set; } = ".local-broker/document-events";
        public int PublishIntervalSeconds { get; set; } = 2;
    }
}
