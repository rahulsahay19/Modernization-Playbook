using HealthCare.Claims.Monolith.Models;
using HealthCare.Claims.Monolith.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/claims")]
public sealed class ClaimsController(ClaimsWorkflowService claims) : ControllerBase
{
    [HttpGet]
    public IActionResult GetClaims() => Ok(claims.GetClaims());

    [HttpGet("{claimNumber}")]
    public IActionResult GetClaim(string claimNumber)
    {
        var claim = claims.GetClaim(claimNumber);
        return claim is null ? NotFound() : Ok(claim);
    }

    [HttpPost]
    public IActionResult SubmitClaim(SubmitClaimRequest request)
    {
        try
        {
            var claim = claims.SubmitClaim(request);
            return CreatedAtAction(nameof(GetClaim), new { claimNumber = claim.ClaimNumber }, claim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{claimNumber}/approve")]
    public IActionResult ApproveClaim(string claimNumber, ApproveClaimRequest request)
    {
        try
        {
            return Ok(claims.ApproveClaim(claimNumber, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{claimNumber}/reject")]
    public IActionResult RejectClaim(string claimNumber, RejectClaimRequest request)
    {
        try
        {
            return Ok(claims.RejectClaim(claimNumber, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

