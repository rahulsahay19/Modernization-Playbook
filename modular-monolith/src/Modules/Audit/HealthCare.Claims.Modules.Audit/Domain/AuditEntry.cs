namespace HealthCare.Claims.Modules.Audit.Domain
{
    public sealed record AuditEntry
    (
        Guid Id,
        string Module,
        string EventName,
        string EntityReference,
        string Summary,
        DateTimeOffset OccurredOn
    );
}
