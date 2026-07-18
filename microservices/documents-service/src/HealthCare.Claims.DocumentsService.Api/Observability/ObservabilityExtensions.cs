using HealthCare.Claims.DocumentsService.Api.Observability;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HealthCare.Claims.DocumentsService.Observability;

public static class ObservabilityExtensions
{
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public static WebApplicationBuilder AddClaimSphereObservability(this WebApplicationBuilder builder, string serviceName)
    {
        var options = builder.Configuration.GetSection("Observability").Get<ObservabilityOptions>() ?? new();
        var serviceVersion = typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: serviceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = builder.Environment.EnvironmentName,
                ["observability.backend"] = options.Backend
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resource);
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(serviceName, serviceVersion: serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(serviceName)
                    .AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
            });

        return builder;
    }

    public static IApplicationBuilder UseClaimSphereCorrelationId(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingCorrelationId)
                && !string.IsNullOrWhiteSpace(existingCorrelationId)
                    ? existingCorrelationId.ToString()
                    : Guid.NewGuid().ToString("N");

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                return Task.CompletedTask;
            });

            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("ClaimSphere.Correlation");

            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                await next();
            }
        });
    }
}