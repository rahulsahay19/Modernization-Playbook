using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Audit;
using HealthCare.Claims.Modules.Claims;
using HealthCare.Claims.Modules.Communications;
using HealthCare.Claims.Modules.Documents;
using HealthCare.Claims.Modules.Membership;
using HealthCare.Claims.Modules.Membership.Domain;
using HealthCare.Claims.Modules.Payments;
using HealthCare.Claims.Modules.Policy;
using HealthCare.Claims.Modules.Policy.Domain;
using HealthCare.Claims.Modules.ProviderNetwork;
using HealthCare.Claims.Modules.ProviderNetwork.Domain;
using HealthCare.Claims.Modules.Reporting;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType<MemberStatus>(CreateStringEnumSchema<MemberStatus>);
    options.MapType<PolicyStatus>(CreateStringEnumSchema<PolicyStatus>);
    options.MapType<ProviderNetworkTier>(CreateStringEnumSchema<ProviderNetworkTier>);
});

IModule[] modules =
[
    new ClaimsModule(),
    new PolicyModule(),
    new MembershipModule(),
    new ProviderNetworkModule(),
    new DocumentsModule(),
    new PaymentsModule(),
    new CommunicationsModule(),
    new AuditModule(),
    new ReportingModule()
];

foreach (var module in modules)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/platform/modules", () => Results.Ok
(modules.Select(module => new
{
    module.Name,
    module.Description
})));

foreach(var module in modules)
{
    module.MapEndpoints(app);
}

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