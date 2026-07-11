using HealthCare.Claims.Modules.Reporting.Domain;

namespace HealthCare.Claims.Modules.Reporting.Application
{
    public sealed record OperationsReport
    (
        int ClaimsSubmitted,
        int ClaimsApproved,
        int ClaimsRejected,
        int DocumentsVerified,
        int PaymentsSettled,
        decimal SubmittedClaimAmount,
        decimal ApprovedClaimAmount,
        decimal SettledPaymentAmount,
        decimal ApprovalRate,
        DateTimeOffset LastUpdatedOn,
        IReadOnlyCollection<RecentBusinessEvent> RecentEvents
    );
}
