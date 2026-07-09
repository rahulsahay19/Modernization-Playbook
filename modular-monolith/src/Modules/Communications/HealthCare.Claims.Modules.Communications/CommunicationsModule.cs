using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Communications.Application;
using HealthCare.Claims.Modules.Communications.Endpoints;
using HealthCare.Claims.Modules.Communications.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Communications
{
    public sealed class CommunicationsModule : IModule
    {
        public string Name => "Communications";

        public string Description => "Email and SMS notification delivery";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
            services.AddScoped<NotificationApplicationService>();
            services.AddScoped<IIntegrationEventHandler<ClaimSubmittedEvent>, NotificationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimApprovedEvent>, NotificationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimRejectedEvent>, NotificationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DocumentVerifiedEvent>, NotificationEventHandler>();
            services.AddScoped<IIntegrationEventHandler<PaymentSettledEvent>, NotificationEventHandler>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            NotificationEndpoints.Map(endpoints);
        }
    }
}
