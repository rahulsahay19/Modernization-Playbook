using HealthCare.Claims.Modules.Reporting.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Reporting.Endpoints
{
    public static class ReportingEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/reports").WithTags("Reporting");
            group.MapGet("/operations", (ReportingApplicationService reporting) =>
                Results.Ok(reporting.GetOperationsReport()));
            group.MapGet("/claims-summary", (ReportingApplicationService reporting) =>
                Results.Ok(reporting.GetClaimsSummary()));
            group.MapGet("/payments-summary", (ReportingApplicationService reporting) =>
                Results.Ok(reporting.GetPaymentsSummary()));
        }
    }
}
