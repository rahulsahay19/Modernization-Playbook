using HealthCare.Claims.Monolith.Services;
using HealthCare.Claims.Monolith.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(PaymentService payments) : ControllerBase
{
    [HttpGet]
    public IActionResult GetPayments() => Ok(payments.GetPayments());

    [HttpPost("claims/{claimNumber}/schedule")]
    public IActionResult SchedulePayment(string claimNumber)
    {
        try
        {
            return Ok(payments.SchedulePayment(claimNumber));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{paymentId}/settle")]
    public IActionResult SettlePayment(string paymentId, SettlePaymentRequest request)
    {
        try
        {
            return Ok(payments.SettlePayment(paymentId, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{paymentId}/fail")]
    public IActionResult FailPayment(string paymentId, FailPaymentRequest request)
    {
        try
        {
            return Ok(payments.FailPayment(paymentId, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
