using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events
{
    public sealed class InProcessEventBus(IServiceScopeFactory scopeFactory) : IEventBus
    {
        public void Publish<TEvent>(TEvent integrationEvent) where TEvent : IIntegrationEvent
        {
            using var scope = scopeFactory.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<TEvent>>();

            foreach(var handler in handlers)
            {
                handler.Handle(integrationEvent);
            }
        }
    }
}
