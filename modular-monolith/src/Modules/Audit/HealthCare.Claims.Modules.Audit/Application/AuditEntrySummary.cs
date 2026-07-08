using HealthCare.Claims.Modules.Audit.Domain;

namespace HealthCare.Claims.Modules.Audit.Application
{
    public sealed record AuditEntrySummary
    (
        Guid Id,
        string Module,
        string EventName,
        string EntityReference,
        string Summary,
        DateTimeOffset OccurredOn
    )
    {
        public static AuditEntrySummary FromEntry(AuditEntry entry) =>
            new(
                entry.Id,
                entry.Module,
                entry.EventName,
                entry.EntityReference,
                entry.Summary,
                entry.OccurredOn
               ); 
    }
}
