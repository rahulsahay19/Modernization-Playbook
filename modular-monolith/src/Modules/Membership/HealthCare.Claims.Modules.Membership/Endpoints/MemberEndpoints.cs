using HealthCare.Claims.Modules.Membership.Application;
using HealthCare.Claims.Modules.Membership.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Membership.Endpoints
{
    public static class MemberEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/members").WithTags("Membership");

            group.MapGet("/", (MembershipApplicationService members, MemberStatus? status, string? policyNumber) =>
            Results.Ok(members.List(status, policyNumber)));

            group.MapGet("/{id:guid}", (Guid id, MembershipApplicationService members) =>
            {
                var member = members.GetById(id);
                return member is null ? Results.NotFound() : Results.Ok(member);
            });
        }
    }
}
