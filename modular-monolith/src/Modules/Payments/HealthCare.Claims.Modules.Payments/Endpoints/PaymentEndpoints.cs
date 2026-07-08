using HealthCare.Claims.Modules.Payments.Application;
using HealthCare.Claims.Modules.Payments.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Payments.Endpoints;

public static class PaymentEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/payments").WithTags("Payments");

        group.MapGet("/", (PaymentApplicationService payments, string? claimNumber, PaymentStatus? status) =>
            Results.Ok(payments.List(claimNumber, status)));

        group.MapGet("/{id:guid}", (Guid id, PaymentApplicationService payments) =>
        {
            var payment = payments.GetById(id);

            return payment is null ? Results.NotFound() : Results.Ok(payment);
        });

        group.MapPost("/", (CreatePaymentInstructionRequest request, PaymentApplicationService payments) =>
        {
            try
            {
                var payment = payments.Create(request);

                return Results.Created($"/api/payments/{payment.Id}", payment);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        group.MapPost("/{id:guid}/schedule", (Guid id, SchedulePaymentRequest request, PaymentApplicationService payments) =>
        {
            try
            {
                var payment = payments.Schedule(id, request);

                return payment is null ? Results.NotFound() : Results.Ok(payment);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        group.MapPost("/{id:guid}/settle", (Guid id, PaymentRemarksRequest request, PaymentApplicationService payments) =>
        {
            try
            {
                var payment = payments.Settle(id, request);

                return payment is null ? Results.NotFound() : Results.Ok(payment);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        group.MapPost("/{id:guid}/fail", (Guid id, PaymentRemarksRequest request, PaymentApplicationService payments) =>
        {
            try
            {
                var payment = payments.MarkFailed(id, request);

                return payment is null ? Results.NotFound() : Results.Ok(payment);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });
    }
}
