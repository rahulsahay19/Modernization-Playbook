using HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Claims.Application;
using HealthCare.Claims.Modules.Claims.Endpoints;
using HealthCare.Claims.Modules.Claims.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Claims
{
    public sealed class ClaimsModule : IModule
    {
        public string Name => "Claims";

        public string Description => "Claim intake, adjudication, and lifecycle ownership.";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClaimRepository, InMemoryClaimRepository>();
            services.AddScoped<IClaimReferenceReader, ClaimReferenceReader>();
            services.AddScoped<ClaimApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            ClaimEndpoints.Map(endpoints);
        }
    }
}
