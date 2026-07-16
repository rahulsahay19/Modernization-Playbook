using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence;

public sealed class EfOutboxStore(DocumentsDbContext dbContext) : IOutboxStore
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        dbContext.OutboxMessages.Add(ToRecord(message));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken)
    {
        var records = await dbContext.OutboxMessages
            .AsNoTracking()
            .Where(message => message.Status == OutboxMessageStatus.Pending)
            .ToArrayAsync(cancellationToken);

        return records
            .OrderBy(message => message.OccurredOn)
            .Take(batchSize)
            .Select(ToDomain)
            .ToArray();
    }

    public async Task MarkPublishedAsync(Guid messageId, DateTimeOffset publishedOn, CancellationToken cancellationToken)
    {
        var record = await dbContext.OutboxMessages.FirstOrDefaultAsync(message => message.Id == messageId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = OutboxMessageStatus.Published;
        record.PublishedOn = publishedOn;
        record.LastError = null;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(Guid messageId, string error, CancellationToken cancellationToken)
    {
        var record = await dbContext.OutboxMessages.FirstOrDefaultAsync(message => message.Id == messageId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Attempts++;
        record.Status = OutboxMessageStatus.Pending;
        record.LastError = error;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> SnapshotAsync(CancellationToken cancellationToken)
    {
        var records = await dbContext.OutboxMessages
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return records
            .OrderByDescending(message => message.OccurredOn)
            .Select(ToDomain)
            .ToArray();
    }

    private static OutboxMessage ToDomain(OutboxMessageRecord record)
    {
        var message = new OutboxMessage(record.Id, record.EventType, record.OccurredOn, record.Payload);

        for (var attempt = 0; attempt < record.Attempts; attempt++)
        {
            message.MarkFailed(record.LastError ?? "Previous publish attempt failed.");
        }

        if (record.Status == OutboxMessageStatus.Published && record.PublishedOn.HasValue)
        {
            message.MarkPublished(record.PublishedOn.Value);
        }

        return message;
    }

    private static OutboxMessageRecord ToRecord(OutboxMessage message) =>
        new()
        {
            Id = message.Id,
            EventType = message.EventType,
            OccurredOn = message.OccurredOn,
            Payload = message.Payload,
            Status = message.Status,
            Attempts = message.Attempts,
            LastError = message.LastError,
            PublishedOn = message.PublishedOn
        };
}
