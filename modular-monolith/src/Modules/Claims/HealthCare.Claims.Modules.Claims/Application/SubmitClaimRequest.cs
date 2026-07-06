using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed record SubmitClaimRequest
    (
        string PolicyNumber,
        string MemberNumber,
        string ProviderCode,
        ClaimType ClaimType,
        DateOnly DateOfService,
        string Diagnosis,
        string Procedure,
        IReadOnlyCollection<SubmitClaimLineRequest> Lines
    );
   

    public sealed record SubmitClaimLineRequest(
        string Description,
        decimal Amount);
}
