using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Reporting.Application;
using HealthCare.Claims.Modules.Reporting.Endpoints;
using HealthCare.Claims.Modules.Reporting.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Reporting
{
    public sealed class ReportingModule : IModule
    {
        public string Name => "Reporting";

        public string Description => "Cross-module operational projections and queries.";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IReportingProjectionRepository, InMemoryReportingProjectionRepository>();
            services.AddScoped<ReportingApplicationService>();
            services.AddScoped<IIntegrationEventHandler<ClaimSubmittedEvent>, ReportingEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimApprovedEvent>, ReportingEventHandler>();
            services.AddScoped<IIntegrationEventHandler<ClaimRejectedEvent>, ReportingEventHandler>();
            services.AddScoped<IIntegrationEventHandler<DocumentVerifiedEvent>, ReportingEventHandler>();
            services.AddScoped<IIntegrationEventHandler<PaymentSettledEvent>, ReportingEventHandler>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            ReportingEndpoints.Map(endpoints);
        }
    }
}
