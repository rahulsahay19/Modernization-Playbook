namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed class IntegrationEventBrokerOptions
{
    public string QueueDirectory { get; set; } = ".local-broker/document-events";

    public int PollIntervalSeconds { get; set; } = 2;
}
