using HealthCare.Claims.Modules.Payments.Domain;

namespace HealthCare.Claims.Modules.Payments.Application
{
    public sealed record PaymentInstructionSummary(
        Guid Id,
        string PaymentNumber,
        string ClaimNumber,
        string PayeeName,
        string BankAccountMasked,
        string IfscCode,
        decimal Amount,
        string Status,
        DateOnly? ScheduledOn,
        string? Remarks,
        DateTimeOffset CreatedOn
        )
    {
        public static PaymentInstructionSummary FromPayment(PaymentInstruction payment) =>
            new(
                payment.Id,
                payment.PaymentNumber,
                payment.ClaimNumber,
                payment.PayeeName,
                payment.BankAccountMasked,
                payment.IfscCode,
                payment.Amount,
                payment.Status.ToString(),
                payment.ScheduledOn,
                payment.Remarks,
                payment.CreatedOn);
    }
    
}
