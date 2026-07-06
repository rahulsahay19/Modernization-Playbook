namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public sealed record ProviderNetworkEligibility
    (
        string ProviderCode,
        string Name,
        string NetworkTier,
        bool isActive,
        bool CashlessEnabled
    );
}
