namespace HealthCare.Claims.Modules.Payments.Application
{
    public sealed record CreatePaymentInstructionRequest(
        string ClaimNumber,
        string PayeeName,
        string BankAccountMasked,
        string IfscCode
        );
    
}
