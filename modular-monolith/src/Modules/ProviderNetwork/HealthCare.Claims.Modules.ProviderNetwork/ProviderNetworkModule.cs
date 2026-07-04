using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.ProviderNetwork
{
    public sealed class ProviderNetworkModule : IModule
    {
        public string Name => "Provider Network";

        public string Description => "Healthcare provider participation, location and network tier.";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            
        }
    }
}
