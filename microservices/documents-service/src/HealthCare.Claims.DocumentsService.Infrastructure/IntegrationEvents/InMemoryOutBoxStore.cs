using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class InMemoryOutBoxStore : IOutboxStore
    {
        private readonly Lock gate = new();
        private readonly List<OutboxMessage> messages = [];
        public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            lock(gate)
            {
                messages.Add(message);
            }
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken)
        {
            lock(gate)
            {
                return Task.FromResult<IReadOnlyCollection<OutboxMessage>>(
                    messages
                    .Where(message => message.Status == OutboxMessageStatus.Pending)
                    .OrderBy(message => message.OccurredOn)
                    .Take(batchSize)
                    .ToArray());
            }
        }

        public Task MarkFailedAsync(Guid messageId, string error, CancellationToken cancellationToken)
        {
            lock(gate)
            {
                messages.FirstOrDefault(message => message.Id == messageId)?.MarkFailed(error);
            }
            return Task.CompletedTask;
        }

        public Task MarkPublishedAsync(Guid messageId, DateTimeOffset publishedOn, CancellationToken cancellationToken)
        {
            lock (gate)
            {
                messages.FirstOrDefault(message => message.Id == messageId)?.MarkPublished(publishedOn);
            }
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OutboxMessage>> SnapshotAsync(CancellationToken cancellationToken)
        {
            lock (gate)
            {
                return Task.FromResult<IReadOnlyCollection<OutboxMessage>>(messages.ToArray());
            }
        }
    }
}
