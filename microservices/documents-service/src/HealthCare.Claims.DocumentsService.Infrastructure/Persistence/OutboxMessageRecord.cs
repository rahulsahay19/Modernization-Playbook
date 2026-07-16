using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence
{
    public sealed class OutboxMessageRecord
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTimeOffset OccurredOn { get; set; }
        public string Payload { get; set; } = string.Empty;
        public OutboxMessageStatus Status { get; set; }
        public int Attempts { get; set; }
        public string? LastError { get; set; }
        public DateTimeOffset? PublishedOn { get; set; }
    }
}
