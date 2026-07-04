using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Payments
{
    public sealed class PaymentsModule : IModule
    {
        public string Name => "Payments";

        public string Description => "Approved-claim payment scheduling and settlement";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            
        }
    }
}
