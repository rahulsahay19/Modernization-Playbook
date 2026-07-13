namespace HealthCare.Claims.Gateway.Proxy
{
    public static class GatewayHttpMethods
    {
        public static readonly string[] All =
            [
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Put,
                HttpMethods.Patch,
                HttpMethods.Delete,
                HttpMethods.Options,
            ];
    }
}
