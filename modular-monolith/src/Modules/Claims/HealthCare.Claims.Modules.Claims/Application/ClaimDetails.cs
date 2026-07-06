using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed record ClaimDetails
        (
            Guid Id,
            string ClaimNumber,
            string PolicyNumber,
            string MemberNumber,
            string ProviderCode,
            string ClaimType,
            string Status,
            DateOnly DateOfService,
            string Diagnosis,
            string Procedure,
            IReadOnlyCollection<ClaimLine> Lines,
            decimal TotalAmount,
            decimal? ApprovedAmount,
            string? DecisionReason,
            DateTimeOffset SubmittedOn
        )
    {
        public static ClaimDetails FromClaim(Claim claim) =>
            new(
                claim.Id,
                claim.ClaimNumber,
                claim.PolicyNumber,
                claim.MemberNumber,
                claim.ProviderCode,
                claim.ClaimType.ToString(),
                claim.Status.ToString(),
                claim.DateOfService,
                claim.Diagnosis,
                claim.Procedure,
                claim.Lines,
                claim.TotalAmount,
                claim.ApprovedAmount,
                claim.DecisionReason,
                claim.SubmittedOn
                );
    }
    
}
