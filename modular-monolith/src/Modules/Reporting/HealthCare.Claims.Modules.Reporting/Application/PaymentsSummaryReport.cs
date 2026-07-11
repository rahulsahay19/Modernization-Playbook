namespace HealthCare.Claims.Modules.Reporting.Application
{
    public sealed record PaymentsSummaryReport
    (
        int SettledPayments,
        decimal SettledAmount,
        decimal AverageSettlementAmount
    );
}
