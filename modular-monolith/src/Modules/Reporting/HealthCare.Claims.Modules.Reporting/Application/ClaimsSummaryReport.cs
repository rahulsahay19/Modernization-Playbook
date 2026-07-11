namespace HealthCare.Claims.Modules.Reporting.Application
{
    public sealed record ClaimsSummaryReport
    (
        int Submitted,
        int Approved,
        int Rejected,
        int Inprogress,
        decimal SubmittedAmount,
        decimal ApprovedAmount,
        decimal ApprovalRate
    );
}
