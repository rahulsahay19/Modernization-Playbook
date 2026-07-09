namespace HealthCare.Claims.Modules.Communications.Domain
{
    public sealed record NotificationMessage
    (
        Guid Id,
        string Recipient,
        NotificationChannel Channel,
        string Subject,
        string Body,
        string SourceEvent,
        NotificationStatus Status,
        DateTimeOffset CreatedOn
    );
   
}
