using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence
{
    public sealed class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options): DbContext(options)
    {
        public DbSet<ClaimDocumentRecord> Documents => Set<ClaimDocumentRecord>();
        public DbSet<OutboxMessageRecord> OutboxMessages => Set<OutboxMessageRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClaimDocumentRecord>(entity =>
            {
                entity.ToTable("ClaimDocuments");
                entity.HasKey(document => document.Id);
                entity.Property(document => document.ClaimNumber).HasMaxLength(32).IsRequired();
                entity.Property(document => document.DocumentType).HasConversion<string>().HasMaxLength(64).IsRequired();
                entity.Property(document => document.FileName).HasMaxLength(256).IsRequired();
                entity.Property(document => document.StorageReference).HasMaxLength(512).IsRequired();
                entity.Property(document => document.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
                entity.Property(document => document.Notes).HasMaxLength(1024);
                entity.HasIndex(document => document.ClaimNumber);
            });
            modelBuilder.Entity<OutboxMessageRecord>(entity =>
            {
                entity.ToTable("OutboxMessages");
                entity.HasKey(message => message.Id);
                entity.Property(message => message.EventType).HasMaxLength(128).IsRequired();
                entity.Property(message => message.Payload).IsRequired();
                entity.Property(message => message.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
                entity.Property(message => message.LastError).HasMaxLength(2048);
                entity.HasIndex(message => new { message.Status, message.OccurredOn });
            });
        }
    }
}
