namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public interface IProviderNetworkReader
    {
        ProviderNetworkEligibility? GetByProviderCode(string providerCode);
    }
}
