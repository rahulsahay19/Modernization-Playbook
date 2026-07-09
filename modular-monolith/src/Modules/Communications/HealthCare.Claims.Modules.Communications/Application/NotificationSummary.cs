using HealthCare.Claims.Modules.Communications.Domain;

namespace HealthCare.Claims.Modules.Communications.Application
{
    public sealed record NotificationSummary
    (
        Guid Id,
        string Recipient,
        string Channel,
        string Subject,
        string Body,
        string SourceEvent,
        string Status,
        DateTimeOffset CreatedOn
    )
    {
        public static NotificationSummary FromNotification(NotificationMessage notification) =>
            new(
                notification.Id,
                notification.Recipient,
                notification.Channel.ToString(),
                notification.Subject,
                notification.Body,
                notification.SourceEvent,
                notification.Status.ToString(),
                notification.CreatedOn
                );
    }
}
