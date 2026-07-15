using System.Text.Json;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed class BrokeredDocumentIntegrationEventConsumer(
    IEventBus eventBus,
    IOptions<IntegrationEventBrokerOptions> options,
    ILogger<BrokeredDocumentIntegrationEventConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, options.Value.PollIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeAvailableMessages();
            await Task.Delay(interval, stoppingToken);
        }
    }

    private void ConsumeAvailableMessages()
    {
        var queueDirectory = BrokerPathResolver.ResolveQueueDirectory(options.Value.QueueDirectory);
        Directory.CreateDirectory(queueDirectory);

        foreach (var messagePath in Directory.EnumerateFiles(queueDirectory, "*.json").OrderBy(path => path))
        {
            try
            {
                var brokeredEvent = JsonSerializer.Deserialize<BrokeredIntegrationEvent>(
                    File.ReadAllText(messagePath),
                    JsonOptions);

                if (brokeredEvent is null)
                {
                    MoveMessage(messagePath, "failed");
                    continue;
                }

                Dispatch(brokeredEvent);
                MoveMessage(messagePath, "processed");
                logger.LogInformation(
                    "Consumed brokered document event {EventType} with message id {MessageId}",
                    brokeredEvent.EventType,
                    brokeredEvent.MessageId);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Could not consume brokered document event file {MessagePath}", messagePath);
                MoveMessage(messagePath, "failed");
            }
        }
    }

    private void Dispatch(BrokeredIntegrationEvent brokeredEvent)
    {
        switch (brokeredEvent.EventType)
        {
            case nameof(DocumentRegisteredIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentRegisteredIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentRegisteredEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName));
                    break;
                }

            case nameof(DocumentVerifiedIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentVerifiedIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentVerifiedEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName));
                    break;
                }

            case nameof(DocumentRejectedIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentRejectedIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentRejectedEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName,
                        integrationEvent.Reason));
                    break;
                }

            default:
                throw new InvalidOperationException($"Unsupported brokered document event type '{brokeredEvent.EventType}'.");
        }
    }

    private static TEvent DeserializePayload<TEvent>(BrokeredIntegrationEvent brokeredEvent)
    {
        var integrationEvent = JsonSerializer.Deserialize<TEvent>(brokeredEvent.Payload, JsonOptions);

        return integrationEvent
            ?? throw new InvalidOperationException($"Could not deserialize payload for '{brokeredEvent.EventType}'.");
    }

    private static void MoveMessage(string messagePath, string folderName)
    {
        var targetDirectory = Path.Combine(Path.GetDirectoryName(messagePath)!, folderName);
        Directory.CreateDirectory(targetDirectory);

        var targetPath = Path.Combine(targetDirectory, Path.GetFileName(messagePath));
        File.Move(messagePath, targetPath, overwrite: true);
    }
}
