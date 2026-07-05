namespace HealthCare.Claims.Modules.ProviderNetwork.Domain
{
    public record HealthCareProvider(
        Guid Id,
        string ProviderCode,
        string Name,
        string Speaciality,
        string City,
        string State,
        ProviderNetworkTier NetworkTier,
        ProviderStatus Status,
        bool CashlessEnabled);
    
}
