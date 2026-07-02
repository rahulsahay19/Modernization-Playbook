using HealthCare.Claims.Monolith.Services;
using HealthCare.Claims.Monolith.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/providers")]
public sealed class ProvidersController(ProviderService providers) : ControllerBase
{
    [HttpGet]
    public IActionResult GetProviders() => Ok(providers.GetProviders());

    [HttpPost]
    public IActionResult CreateProvider(CreateProviderRequest request)
    {
        try
        {
            return CreatedAtAction(nameof(GetProviders), providers.CreateProvider(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{providerId}")]
    public IActionResult UpdateProvider(string providerId, UpdateProviderRequest request)
    {
        try
        {
            return Ok(providers.UpdateProvider(providerId, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{providerId}/deactivate")]
    public IActionResult DeactivateProvider(string providerId)
    {
        try
        {
            return Ok(providers.DeactivateProvider(providerId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
