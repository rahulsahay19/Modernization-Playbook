using HealthCare.Claims.DocumentsService.Api.Endpoints;
using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Handlers.Documents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;
using HealthCare.Claims.DocumentsService.Domain.Enums;
using HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Infrastructure.Repositories;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<IClaimDocumentRepository, InMemoryClaimDocumentRepository>();
builder.Services.Configure<IntegrationEventRelayOptions>(builder.Configuration.GetSection("IntegrationEventRelay"));
builder.Services.AddHttpClient<IIntegrationEventPublisher, HttpIntegrationEventPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Service-Name"] = "DocumentsService";
    await next();
});

app.MapGet("/", () => Results.Redirect("/api/documents"));
app.MapServiceInfoEndpoints();
app.MapDocumentEndpoints();

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
