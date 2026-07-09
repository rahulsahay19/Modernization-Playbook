using HealthCare.Claims.Modules.Audit.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Audit.Endpoints
{
    public static class AuditEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/audit").WithTags("Audit");
            group.MapGet("/", (AuditApplicationService audit, string? module, string? eventName) =>
            Results.Ok(audit.List(module, eventName)));
        }
    }
}
