using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.ProviderNetwork.Application;
using HealthCare.Claims.Modules.ProviderNetwork.Endpoints;
using HealthCare.Claims.Modules.ProviderNetwork.Infrastructure;
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
            services.AddSingleton<IProviderRepository, InMemoryProviderRepository>();
            services.AddScoped<ProviderApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            ProviderEndpoints.Map(endpoints);
        }
    }
}
