using HealthCare.Claims.DocumentsService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence
{
    public static class DocumentsDatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
            var databasePath = dbContext.Database.GetDbConnection().DataSource;

            if (!string.IsNullOrWhiteSpace(databasePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
            }

            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            if (await dbContext.Documents.AnyAsync(cancellationToken))
            {
                return;
            }
            dbContext.Documents.AddRange(
                 new ClaimDocumentRecord
                 {
                     Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                     ClaimNumber = "CLM-202606270001",
                     DocumentType = ClaimDocumentType.DischargeSummary,
                     FileName = "discharge-summary-clm-202606270001.pdf",
                     StorageReference = "local://claims/CLM-202606270001/discharge-summary.pdf",
                     Status = ClaimDocumentStatus.Verified,
                     Notes = "Hospital discharge summary verified.",
                     ReceivedOn = DateTimeOffset.Parse("2026-06-30T12:20:00+00:00")
                 },
            new ClaimDocumentRecord
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                ClaimNumber = "CLM-202606270001",
                DocumentType = ClaimDocumentType.FinalBill,
                FileName = "final-bill-clm-202606270001.pdf",
                StorageReference = "local://claims/CLM-202606270001/final-bill.pdf",
                Status = ClaimDocumentStatus.Received,
                Notes = "Pending billing team verification.",
                ReceivedOn = DateTimeOffset.Parse("2026-06-30T12:23:22.2029999+00:00")
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
