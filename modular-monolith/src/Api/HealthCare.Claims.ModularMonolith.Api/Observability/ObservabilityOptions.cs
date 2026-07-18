namespace HealthCare.Claims.ModularMonolith.Api.Observability
{
    public sealed class ObservabilityOptions
    {
        public string Backend { get; set; } = "Aspire";

        public string OtlpEndpoint { get; set; } = "http://localhost:18889";
    }
}
