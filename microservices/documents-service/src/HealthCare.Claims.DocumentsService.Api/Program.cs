using HealthCare.Claims.DocumentsService.Api.Endpoints;
using HealthCare.Claims.DocumentsService.Api.Endpoints.Persistence;
using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Handlers.Documents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;
using HealthCare.Claims.DocumentsService.Domain.Enums;
using HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Infrastructure.Persistence;
using HealthCare.Claims.DocumentsService.Observability;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddClaimSphereObservability("claimsphere.documents-service");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType<ClaimDocumentStatus>(CreateStringEnumSchema<ClaimDocumentStatus>);
    options.MapType<ClaimDocumentType>(CreateStringEnumSchema<ClaimDocumentType>);
});

builder.Services.AddScoped<IQueryHandler<ListDocumentsQuery, IReadOnlyCollection<DocumentResponse>>, ListDocumentsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetDocumentQuery, DocumentResponse?>, GetDocumentQueryHandler>();
builder.Services.AddScoped<ICommandHandler<RegisterDocumentCommand, DocumentCommandResult>, RegisterDocumentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<VerifyDocumentCommand, DocumentCommandResult>, VerifyDocumentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RejectDocumentCommand, DocumentCommandResult>, RejectDocumentCommandHandler>();

var documentsConnectionString = DatabasePathResolver.ResolveSqliteConnectionString(
    builder.Configuration.GetConnectionString("DocumentsDatabase"));
builder.Services.AddDbContext<DocumentsDbContext>(options =>
    options.UseSqlite(documentsConnectionString));

builder.Services.AddScoped<IClaimDocumentRepository, EfClaimDocumentRepository>();
builder.Services.Configure<IntegrationEventBrokerOptions>(builder.Configuration.GetSection("IntegrationEventBroker"));
builder.Services.AddScoped<IOutboxStore, EfOutboxStore>();
builder.Services.AddSingleton<IIntegrationEventBroker>(services =>
{
    var options = services.GetRequiredService<IOptions<IntegrationEventBrokerOptions>>().Value;
    return string.Equals(options.Transport, "LocalFile", StringComparison.OrdinalIgnoreCase)
    ? new LocalFileIntegrationEventBroker(services.GetRequiredService<IOptions<IntegrationEventBrokerOptions>>())
    : new RabbitMqIntegrationEventBroker(services.GetRequiredService<IOptions<IntegrationEventBrokerOptions>>());
});
builder.Services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();
builder.Services.AddHostedService<OutboxPublisherBackgroundService>();

var app = builder.Build();
await DocumentsDatabaseInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseClaimSphereCorrelationId();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Service-Name"] = "DocumentsService";
    await next();
});

app.MapGet("/", () => Results.Redirect("/api/documents"));
app.MapServiceInfoEndpoints();
app.MapDocumentEndpoints();
app.MapOutboxEndpoints();
app.MapStorageEndPoints();
app.Run();

static OpenApiSchema CreateStringEnumSchema<TEnum>()
    where TEnum : struct, Enum =>
    new()
    {
        Type = JsonSchemaType.String,
        Enum = Enum.GetNames<TEnum>()
            .Select(name => JsonValue.Create(name)!)
            .Cast<JsonNode>()
            .ToList()
    };
