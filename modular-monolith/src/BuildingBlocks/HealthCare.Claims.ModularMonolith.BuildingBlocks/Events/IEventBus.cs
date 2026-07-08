namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent integrationEvent)
            where TEvent : IIntegrationEvent; 
    }
}
