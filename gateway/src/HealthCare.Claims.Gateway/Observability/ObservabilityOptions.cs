namespace HealthCare.Claims.Gateway.Observability
{
    public sealed class ObservabilityOptions
    {
        public string Backend { get; set; } = "Aspire";
        public string OtlpEndpoint { get; set; } = "http://localhost:18889";
    }
}
