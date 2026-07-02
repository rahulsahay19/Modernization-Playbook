using HealthCare.Claims.Monolith.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/audit")]
public sealed class AuditController(AuditService audit) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAuditEntries() => Ok(audit.GetAuditEntries());
}

