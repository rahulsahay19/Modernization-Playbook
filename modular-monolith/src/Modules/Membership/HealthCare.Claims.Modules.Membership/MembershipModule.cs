using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Membership.Application;
using HealthCare.Claims.Modules.Membership.Endpoints;
using HealthCare.Claims.Modules.Membership.Infrastructure;
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
            services.AddSingleton<IMemberRepository, InMemoryMemberRepository>();
            services.AddScoped<MembershipApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            MemberEndpoints.Map(endpoints);
        }
    }
}
