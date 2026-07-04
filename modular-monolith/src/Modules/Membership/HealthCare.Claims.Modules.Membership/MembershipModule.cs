using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Membership
{
    public sealed class MembershipModule : IModule
    {
        public string Name => "Membership";

        public string Description => "Member identity, enrollment, eligibility and contact details";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            
        }
    }
}
