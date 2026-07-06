using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed record ClaimSummary(
        Guid Id,
        string ClaimNumber,
        string PolicyNumber,
        string MemberNumber,
        string ProviderCode,
        string ClaimType,
        string Status,
        decimal TotalAmount,
        decimal? ApprovedAmount,
        DateOnly DateOfService,
        DateTimeOffset SubmittedOn)

    {
        public static ClaimSummary FromClaim(Claim claim) =>
            new(
                claim.Id,
                claim.ClaimNumber,
                claim.PolicyNumber,
                claim.MemberNumber,
                claim.ProviderCode,
                claim.ClaimType.ToString(),
                claim.Status.ToString(),
                claim.TotalAmount,
                claim.ApprovedAmount,
                claim.DateOfService,
                claim.SubmittedOn
               );
    }

}
