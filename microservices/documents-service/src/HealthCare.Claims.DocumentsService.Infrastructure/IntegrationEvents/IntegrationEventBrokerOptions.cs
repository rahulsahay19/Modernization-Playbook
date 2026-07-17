namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class IntegrationEventBrokerOptions
    {
        public string Transport { get; set; } = "RabbitMq";
        public string QueueDirectory { get; set; } = ".local-broker/document-events";
        public int PublishIntervalSeconds { get; set; } = 2;
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = "claims.integration.events";
        public string RoutingKey { get; set; } = "documents.events";
        public string QueueName { get; set; } = "modular-monolith.documents";
    }
}
