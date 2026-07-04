using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Audit;
using HealthCare.Claims.Modules.Claims;
using HealthCare.Claims.Modules.Communications;
using HealthCare.Claims.Modules.Documents;
using HealthCare.Claims.Modules.Membership;
using HealthCare.Claims.Modules.Payments;
using HealthCare.Claims.Modules.Policy;
using HealthCare.Claims.Modules.ProviderNetwork;
using HealthCare.Claims.Modules.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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