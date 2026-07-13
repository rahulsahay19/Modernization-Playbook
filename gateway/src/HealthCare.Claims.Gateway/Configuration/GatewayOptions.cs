namespace HealthCare.Claims.Gateway.Configuration
{
    public sealed class GatewayOptions
    {
        public List<StranglerRoute> Routes { get; set; } = []; 
    }
}
