using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Policy.Application;
using HealthCare.Claims.Modules.Policy.Endpoints;
using HealthCare.Claims.Modules.Policy.Infrastructure;
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
            services.AddSingleton<IPolicyRepository, InMemoryPolicyRepository>();
            services.AddScoped<PolicyApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            PolicyEndPoints.Map(endpoints);
        }
    }
}
