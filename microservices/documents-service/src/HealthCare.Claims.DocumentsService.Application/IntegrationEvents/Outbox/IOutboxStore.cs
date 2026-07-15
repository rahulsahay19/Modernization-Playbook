namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox
{
    public interface IOutboxStore
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);
        Task MarkPublishedAsync(Guid messageId, DateTimeOffset publishedOn, CancellationToken cancellationToken);
        Task MarkFailedAsync(Guid messageId, string error, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<OutboxMessage>> SnapshotAsync(CancellationToken cancellationToken);
    }
}
