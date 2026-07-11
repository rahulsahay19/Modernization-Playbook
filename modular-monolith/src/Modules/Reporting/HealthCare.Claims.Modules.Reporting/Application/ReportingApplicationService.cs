namespace HealthCare.Claims.Modules.Reporting.Application;

public sealed class ReportingApplicationService(IReportingProjectionRepository projections)
{
    public OperationsReport GetOperationsReport()
    {
        var projection = projections.GetProjection();
        var recentEvents = projections.ListRecentEvents();

        return new OperationsReport(
            projection.ClaimsSubmitted,
            projection.ClaimsApproved,
            projection.ClaimsRejected,
            projection.DocumentsVerified,
            projection.PaymentsSettled,
            projection.SubmittedClaimAmount,
            projection.ApprovedClaimAmount,
            projection.SettledPaymentAmount,
            CalculateRate(projection.ClaimsApproved, projection.ClaimsSubmitted),
            projection.LastUpdatedOn,
            recentEvents);
    }

    public ClaimsSummaryReport GetClaimsSummary()
    {
        var projection = projections.GetProjection();
        var inProgress = Math.Max(0, projection.ClaimsSubmitted - projection.ClaimsApproved - projection.ClaimsRejected);

        return new ClaimsSummaryReport(
            projection.ClaimsSubmitted,
            projection.ClaimsApproved,
            projection.ClaimsRejected,
            inProgress,
            projection.SubmittedClaimAmount,
            projection.ApprovedClaimAmount,
            CalculateRate(projection.ClaimsApproved, projection.ClaimsSubmitted));
    }

    public PaymentsSummaryReport GetPaymentsSummary()
    {
        var projection = projections.GetProjection();

        return new PaymentsSummaryReport(
            projection.PaymentsSettled,
            projection.SettledPaymentAmount,
            projection.PaymentsSettled == 0 ? 0m : decimal.Round(projection.SettledPaymentAmount / projection.PaymentsSettled, 2));
    }

    private static decimal CalculateRate(int numerator, int denominator) =>
        denominator == 0 ? 0m : decimal.Round((decimal)numerator / denominator * 100m, 2);
}
