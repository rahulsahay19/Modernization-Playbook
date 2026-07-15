using System.Text.Json;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;

public sealed class LocalFileIntegrationEventBroker(IOptions<IntegrationEventBrokerOptions> options) : IIntegrationEventBroker
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var queueDirectory = BrokerPathResolver.ResolveQueueDirectory(options.Value.QueueDirectory);
        Directory.CreateDirectory(queueDirectory);

        var brokeredEvent = new BrokeredIntegrationEvent(
            message.Id,
            message.EventType,
            message.OccurredOn,
            message.Payload);

        var finalPath = Path.Combine(queueDirectory, $"{message.OccurredOn:yyyyMMddHHmmssfff}-{message.Id}.json");
        var tempPath = finalPath + ".tmp";

        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, brokeredEvent, JsonOptions, cancellationToken);
        }

        File.Move(tempPath, finalPath, overwrite: true);
    }
}
