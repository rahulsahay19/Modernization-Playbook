namespace HealthCare.Claims.Modules.Payments.Application
{
    public sealed record SchedulePaymentRequest(
        DateOnly ScheduledOn,
        string? Remarks
        );
    
}
