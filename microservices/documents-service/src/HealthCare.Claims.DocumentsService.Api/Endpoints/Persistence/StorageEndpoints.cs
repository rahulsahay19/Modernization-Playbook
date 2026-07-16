using HealthCare.Claims.DocumentsService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints.Persistence
{
    public static class StorageEndpoints
    {
        public static IEndpointRouteBuilder MapStorageEndPoints(this IEndpointRouteBuilder endpoints) 
        {
            endpoints.MapGet("/api/storage", async (
                DocumentsDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var databasePath = dbContext.Database.GetDbConnection().DataSource;
                return Results.Ok(new
                {
                    storage = "SQLite service-owned database",
                    databasePath,
                    documents = await dbContext.Documents.CountAsync(cancellationToken),
                    outboxMessages = await dbContext.OutboxMessages.CountAsync(cancellationToken),
                    pendingOutboxMessages = await dbContext.OutboxMessages.CountAsync(
                        message => message.Status == Application.IntegrationEvents.Outbox.OutboxMessageStatus.Pending,
                        cancellationToken)
                });
            }).WithTags("Persistence");
            return endpoints;
        }
    }
}
