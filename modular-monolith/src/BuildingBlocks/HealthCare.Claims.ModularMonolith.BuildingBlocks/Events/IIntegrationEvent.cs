namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTimeOffset OccurredOn { get; }
    }
}
