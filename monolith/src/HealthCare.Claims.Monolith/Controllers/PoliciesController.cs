using HealthCare.Claims.Monolith.Services;
using HealthCare.Claims.Monolith.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PoliciesController(PolicyService policies) : ControllerBase
{
    [HttpGet]
    public IActionResult GetPolicies() => Ok(policies.GetPolicies());

    [HttpPost]
    public IActionResult CreatePolicy(CreatePolicyRequest request)
    {
        try
        {
            return CreatedAtAction(nameof(GetPolicies), policies.CreatePolicy(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{policyNumber}")]
    public IActionResult UpdatePolicy(string policyNumber, UpdatePolicyRequest request)
    {
        try
        {
            return Ok(policies.UpdatePolicy(policyNumber, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{policyNumber}/deactivate")]
    public IActionResult DeactivatePolicy(string policyNumber)
    {
        try
        {
            return Ok(policies.DeactivatePolicy(policyNumber));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
