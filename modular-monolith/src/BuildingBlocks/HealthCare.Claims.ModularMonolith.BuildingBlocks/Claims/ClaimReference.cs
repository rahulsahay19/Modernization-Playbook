namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims
{
    public sealed record ClaimReference(
        Guid Id,
        string ClaimNumber,
        string PolicyNumber,
        string MemberNumber,
        string ProviderCode,
        string Status,
        decimal TotalAmount,
        decimal? ApprovedAmount);
}
