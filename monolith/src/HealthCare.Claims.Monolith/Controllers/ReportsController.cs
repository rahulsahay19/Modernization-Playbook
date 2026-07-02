using HealthCare.Claims.Monolith.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(ReportingService reports) : ControllerBase
{
    [HttpGet("operations")]
    public IActionResult GetOperationsDashboard([FromQuery] DateOnly? from, [FromQuery] DateOnly? to) =>
        Ok(reports.GetOperationalDashboard(from, to));
}
