using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
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
            
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            
        }
    }
}
