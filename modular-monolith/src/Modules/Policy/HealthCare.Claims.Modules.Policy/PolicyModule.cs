using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Policy
{
    public sealed class PolicyModule : IModule
    {
        public string Name => "Policy";

        public string Description => "Insurance products, coverage limits and policy status";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            
        }
    }
}
