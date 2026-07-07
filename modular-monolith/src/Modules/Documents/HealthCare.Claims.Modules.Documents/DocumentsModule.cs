using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Documents.Application;
using HealthCare.Claims.Modules.Documents.Endpoints;
using HealthCare.Claims.Modules.Documents.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Documents
{
    public sealed class DocumentsModule : IModule
    {
        public string Name => "Documents";

        public string Description => "Claim evidence metadata, storage and verification";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClaimDocumentRepository, InMemoryClaimDocumentRepository>();
            services.AddScoped<DocumentApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            DocumentEndpoints.Map(endpoints);
        }
    }
}
