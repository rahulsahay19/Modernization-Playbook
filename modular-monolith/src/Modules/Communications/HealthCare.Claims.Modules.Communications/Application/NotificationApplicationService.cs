namespace HealthCare.Claims.Modules.Communications.Application
{
    public sealed class NotificationApplicationService(INotificationRepository notifications)
    {
        public IReadOnlyCollection<NotificationSummary> List(string? recipient = null, string? sourceEvent = null)
        {
            var query = notifications.List().AsEnumerable();
            if(!string.IsNullOrWhiteSpace(recipient))
            {
                query = query.Where(notification =>
                        notification.Recipient.Contains(recipient, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrWhiteSpace(sourceEvent))
            {
                query = query.Where(notification =>
                            string.Equals(notification.SourceEvent, sourceEvent, StringComparison.OrdinalIgnoreCase));
            }
            return query
                  .OrderByDescending(notification => notification.CreatedOn)
                  .Select(NotificationSummary.FromNotification)
                  .ToArray();
        }
    }
}
