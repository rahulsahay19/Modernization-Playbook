using HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed class ClaimReferenceReader(IClaimRepository claims) : IClaimReferenceReader
    {
        public ClaimReference? GetByClaimNumber(string claimNumber)
        {
            var claim = claims.List()
                        .FirstOrDefault(candidate =>
                            string.Equals(candidate.ClaimNumber, claimNumber, StringComparison.OrdinalIgnoreCase));
            return claim is null
                ? null
                : new ClaimReference(
                    claim.Id,
                    claim.ClaimNumber,
                    claim.PolicyNumber,
                    claim.MemberNumber,
                    claim.ProviderCode,
                    claim.Status.ToString(),
                    claim.TotalAmount,
                    claim.ApprovedAmount);
        }
    }
}
