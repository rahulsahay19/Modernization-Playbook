namespace HealthCare.Claims.Modules.Reporting.Domain
{
    public sealed record RecentBusinessEvent
    (
        string EventName,
        string EntityReference,
        string Summary,
        DateTimeOffset OccurredOn
    );
}
