using HealthCare.Claims.Monolith.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController(NotificationService notifications) : ControllerBase
{
    [HttpGet]
    public IActionResult GetNotifications() => Ok(notifications.GetNotifications());
}

