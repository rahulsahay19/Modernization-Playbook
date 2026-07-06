namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed record ClaimDecisionRequest
    (
        decimal? ApprovedAmount,
        string Reason
        );
}
