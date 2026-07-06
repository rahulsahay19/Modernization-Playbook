using HealthCare.Claims.Modules.Claims.Application;
using HealthCare.Claims.Modules.Claims.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Claims.Endpoints
{
    public static class ClaimEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/claims").WithTags("Claims");

            group.MapGet("/", (ClaimApplicationService claims, ClaimStatus? status, string? memberNumber) =>
                Results.Ok(claims.List(status, memberNumber)));

            group.MapGet("/{id:guid}", (Guid id, ClaimApplicationService claims) =>
            {
                var claim = claims.GetById(id);
                return claim is null ? Results.NotFound() : Results.Ok(claim);
            });

            group.MapPost("/", (SubmitClaimRequest request, ClaimApplicationService claims) =>
            {
                try 
                {
                    var claim = claims.Submit(request);
                    return Results.Created($"/api/claims/{claim.Id}", claim);
                }
                catch(InvalidOperationException ex) 
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            group.MapPost("/{id:guid}/review", (Guid id, ClaimApplicationService claims) =>
            {
                try
                {
                    var claim = claims.MarkUnderReview(id);
                    return claim is null ? Results.NotFound() : Results.Ok(claim);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
            group.MapPost("/{id:guid}/approve", (Guid id, ClaimDecisionRequest request, ClaimApplicationService claims) =>
            {
                try
                {
                    var claim = claims.Approve(id, request);
                    return claim is null ? Results.NotFound() : Results.Ok(claim);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
            group.MapPost("/{id:guid}/reject", (Guid id, ClaimDecisionRequest request, ClaimApplicationService claims) =>
            {
                try
                {
                    var claim = claims.Reject(id, request);
                    return claim is null ? Results.NotFound() : Results.Ok(claim);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}
