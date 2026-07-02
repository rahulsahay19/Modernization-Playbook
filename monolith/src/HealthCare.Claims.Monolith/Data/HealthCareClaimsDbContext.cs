using HealthCare.Claims.Monolith.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.Monolith.Data;

public sealed class HealthCareClaimsDbContext(DbContextOptions<HealthCareClaimsDbContext> options) : DbContext(options)
{
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimDocument> Documents => Set<ClaimDocument>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<NotificationMessage> Notifications => Set<NotificationMessage>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Policy>().HasKey(policy => policy.PolicyNumber);
        modelBuilder.Entity<Member>().HasKey(member => member.MemberId);
        modelBuilder.Entity<Provider>().HasKey(provider => provider.ProviderId);
        modelBuilder.Entity<Claim>().HasKey(claim => claim.ClaimNumber);
        modelBuilder.Entity<ClaimDocument>().HasKey(document => document.DocumentId);
        modelBuilder.Entity<Payment>().HasKey(payment => payment.PaymentId);
        modelBuilder.Entity<NotificationMessage>().HasKey(notification => notification.NotificationId);
        modelBuilder.Entity<AuditEntry>().HasKey(audit => audit.AuditId);

        modelBuilder.Entity<Claim>().Ignore(claim => claim.DocumentIds);
        modelBuilder.Entity<Claim>().OwnsMany(claim => claim.Lines, lines =>
        {
            lines.ToTable("ClaimLines");
            lines.Property<int>("Id");
            lines.HasKey("Id");
            lines.WithOwner().HasForeignKey("ClaimNumber");
        });

        modelBuilder.Entity<Member>()
            .HasOne<Policy>()
            .WithMany()
            .HasForeignKey(member => member.PolicyNumber)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Claim>()
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(claim => claim.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Claim>()
            .HasOne<Provider>()
            .WithMany()
            .HasForeignKey(claim => claim.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClaimDocument>()
            .HasOne<Claim>()
            .WithMany()
            .HasForeignKey(document => document.ClaimNumber)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne<Claim>()
            .WithMany()
            .HasForeignKey(payment => payment.ClaimNumber)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
