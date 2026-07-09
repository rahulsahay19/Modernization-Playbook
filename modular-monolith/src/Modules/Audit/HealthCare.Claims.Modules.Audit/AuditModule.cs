using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Audit.Application;
using HealthCare.Claims.Modules.Audit.Endpoints;
using HealthCare.Claims.Modules.Audit.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Audit
{
    public sealed class AuditModule : IModule
    {
        public string Name => "Audit";

        public string Description => "Immutable business-action history and accoutability";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuditEntryRepository, InMemoryAuditEntryRepository>();
            services.AddScoped<AuditApplicationService>();
            services.AddScoped<IIntegrationEventHandler<ClaimSubmittedEvent>, AuditEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimApprovedEvent>, AuditEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimRejectedEvent>, AuditEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DocumentVerifiedEvent>, AuditEventHandler>();
            services.AddScoped<IIntegrationEventHandler<PaymentSettledEvent>, AuditEventHandler>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AuditEndpoints.Map(endpoints);
        }
    }
}
