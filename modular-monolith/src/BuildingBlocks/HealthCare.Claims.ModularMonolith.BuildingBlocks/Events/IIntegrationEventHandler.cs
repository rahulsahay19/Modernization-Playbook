namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events
{
    public interface IIntegrationEventHandler<in TEvent>
        where TEvent : IIntegrationEvent
    {
        void Handle(TEvent integrationEvent);
    }
}
