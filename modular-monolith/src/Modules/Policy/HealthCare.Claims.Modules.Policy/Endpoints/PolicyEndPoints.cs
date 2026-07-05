using HealthCare.Claims.Modules.Policy.Application;
using HealthCare.Claims.Modules.Policy.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Policy.Endpoints
{
    public static class PolicyEndPoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/policies").WithTags("Policy");

            //GET /api/policies
            //GET /api/policies?status=Active
            group.MapGet("/", (PolicyApplicationService policies, PolicyStatus? status) =>
            Results.Ok(policies.List(status)));

            //GET /api/policies/{id}
            group.MapGet("/{id:guid}", (Guid id, PolicyApplicationService policies) =>
            {
                var policy = policies.GetById(id);
                return policy is null ? Results.NotFound() : Results.Ok(policy);
            });
        }
    }
}
