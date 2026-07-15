namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox
{
    public sealed class OutboxMessage
    {
        public OutboxMessage(Guid id, string eventType, DateTimeOffset occurredOn, string payload)
        {
            Id = id;
            EventType = eventType;
            OccurredOn = occurredOn;
            Payload = payload;
        }
        public Guid Id { get; }
        public string EventType { get; }
        public DateTimeOffset OccurredOn { get; }
        public string Payload { get; }
        public OutboxMessageStatus Status { get; private set; } = OutboxMessageStatus.Pending;
        public int Attempts { get; private set; }
        public string? LastError { get; private set; }
        public DateTimeOffset? PublishedOn { get; private set; }

        public void MarkPublished(DateTimeOffset publishedOn)
        {
            Status = OutboxMessageStatus.Published;
            PublishedOn = publishedOn;
            LastError = null;
        }
        public void MarkFailed(string error)
        {
            Attempts++;
            Status = OutboxMessageStatus.Pending;
            LastError = error;
        }
    }
}
