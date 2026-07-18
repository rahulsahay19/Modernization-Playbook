using HealthCare.Claims.Gateway.Configuration;
using HealthCare.Claims.Gateway.Endpoints;
using HealthCare.Claims.Gateway.Observability;
using HealthCare.Claims.Gateway.Proxy;

var builder = WebApplication.CreateBuilder(args);
builder.AddClaimSphereObservability("claimsphere.gateway");
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.AddHttpClient("strangler-gateway");
builder.Services.AddSingleton<StranglerRouteSelector>();
builder.Services.AddScoped<GatewayProxy>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
app.UseClaimSphereCorrelationId();
app.UseCors("FrontendCors");
app.MapGatewayEndpoints();
app.Run();