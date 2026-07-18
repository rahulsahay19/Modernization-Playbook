using System.Text.Json;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed class BrokeredDocumentIntegrationEventConsumer(
    DocumentIntegrationEventDispatcher dispatcher,
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

                dispatcher.Dispatch(brokeredEvent);
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

    private static void MoveMessage(string messagePath, string folderName)
    {
        var targetDirectory = Path.Combine(Path.GetDirectoryName(messagePath)!, folderName);
        Directory.CreateDirectory(targetDirectory);

        var targetPath = Path.Combine(targetDirectory, Path.GetFileName(messagePath));
        File.Move(messagePath, targetPath, overwrite: true);
    }
}
