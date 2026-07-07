namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims
{
    public interface IClaimReferenceReader
    {
        ClaimReference? GetByClaimNumber(string claimNumber);
    }
}
