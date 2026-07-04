using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules
{
    public interface IModule
    {
        string Name { get; }
        string Description { get; }

        void AddServices(IServiceCollection services, IConfiguration configuration);
        void MapEndpoints(IEndpointRouteBuilder endpoints);
    }
}
