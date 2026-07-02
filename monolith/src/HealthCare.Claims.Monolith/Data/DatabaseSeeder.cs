using HealthCare.Claims.Monolith.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.Monolith.Data;

public static class DatabaseSeeder
{
    public static void Seed(HealthCareClaimsDbContext database)
    {
        if (database.Policies.Any())
        {
            return;
        }

        database.Policies.AddRange(
            new Policy
            {
                PolicyNumber = "POL-1001",
                InsuranceProviderName = "Star Health and Allied Insurance Co. Ltd.",
                PlanName = "Family Health Optima Insurance Plan",
                AnnualLimit = 250000,
                Deductible = 5000,
                IsActive = true
            },
            new Policy
            {
                PolicyNumber = "POL-2002",
                InsuranceProviderName = "HDFC ERGO General Insurance Co. Ltd.",
                PlanName = "Optima Secure Health Insurance",
                AnnualLimit = 500000,
                Deductible = 2500,
                IsActive = true
            });

        database.Members.AddRange(
            new Member
            {
                MemberId = "MEM-1001",
                FullName = "Aarav Mehta",
                PolicyNumber = "POL-1001",
                DateOfBirth = new DateOnly(1988, 4, 12),
                Email = "aarav.mehta@example.com",
                MobileNumber = "+919876543210",
                IsActive = true
            },
            new Member
            {
                MemberId = "MEM-2002",
                FullName = "Maya Iyer",
                PolicyNumber = "POL-2002",
                DateOfBirth = new DateOnly(1992, 9, 2),
                Email = "maya.iyer@example.com",
                MobileNumber = "+919812345678",
                IsActive = true
            });

        database.Providers.AddRange(
            new Provider
            {
                ProviderId = "PRV-101",
                Name = "Apollo Hospitals",
                NetworkTier = "Gold",
                City = "Chennai",
                IsActive = true
            },
            new Provider
            {
                ProviderId = "PRV-202",
                Name = "Dr. Lal PathLabs",
                NetworkTier = "Silver",
                City = "Gurugram",
                IsActive = true
            });

        database.Claims.Add(new Claim
        {
            ClaimNumber = "CLM-90001",
            MemberId = "MEM-1001",
            ProviderId = "PRV-101",
            ServiceDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-12)),
            RequestedAmount = 18500,
            ApprovedAmount = 0,
            Status = ClaimStatus.PendingDocuments,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            Lines =
            [
                new ClaimLine { Code = "ER-100", Description = "Emergency consultation", Amount = 8500 },
                new ClaimLine { Code = "LAB-210", Description = "Blood panel", Amount = 10000 }
            ]
        });

        database.Documents.Add(new ClaimDocument
        {
            DocumentId = "DOC-70001",
            ClaimNumber = "CLM-90001",
            FileName = "er-bill.pdf",
            DocumentType = "HospitalBill",
            StoragePath = "/SimulatedDocumentStore/CLM-90001/er-bill.pdf",
            Status = DocumentStatus.Verified,
            UploadedAt = DateTimeOffset.UtcNow.AddDays(-9)
        });

        database.AuditEntries.Add(new AuditEntry
        {
            AuditId = "AUD-10001",
            EntityType = "Claim",
            EntityId = "CLM-90001",
            Action = "Seeded",
            Description = "Seeded the initial claim for monolith walkthrough.",
            PerformedBy = "legacy-system",
            PerformedAt = DateTimeOffset.UtcNow.AddDays(-9)
        });

        database.SaveChanges();
        database.ChangeTracker.Clear();
    }
}
