using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence
{
    public sealed class EfClaimDocumentRepository(DocumentsDbContext dbContext) : IClaimDocumentRepository
    {
        public void Add(ClaimDocument document)
        {
            dbContext.Documents.Add(ToRecord(document));
        }

        public ClaimDocument? GetById(Guid id)
        {
            var record = dbContext.Documents.AsNoTracking().FirstOrDefault(document => document.Id == id);
            return record is null ? null : ToDomain(record); 
        }

        public IReadOnlyCollection<ClaimDocument> List(string? claimNumber = null)
        {
            var query = dbContext.Documents.AsNoTracking();
            if(!string.IsNullOrWhiteSpace(claimNumber))
            {
                query = query.Where(document => document.ClaimNumber == claimNumber);
            }
            return query
                .AsEnumerable()
                .OrderByDescending(document => document.ClaimNumber)
                .Select(ToDomain)
                .ToArray();
        }

        public void Update(ClaimDocument document)
        {
            var record = dbContext.Documents.FirstOrDefault(existing => existing.Id == document.Id)
                ?? throw new InvalidOperationException($"Document '{document.Id}' was not found.");
            record.Status = document.Status;
            record.Notes = document.Notes;
        }

        private static ClaimDocument ToDomain(ClaimDocumentRecord record) =>
            new(
                record.Id,
                record.ClaimNumber,
                record.DocumentType,
                record.FileName,
                record.StorageReference,
                record.Status,
                record.Notes,
                record.ReceivedOn);

        private static ClaimDocumentRecord ToRecord(ClaimDocument document) =>
            new()
            {
                Id = document.Id,
                ClaimNumber = document.ClaimNumber,
                DocumentType = document.DocumentType,
                FileName = document.FileName,
                StorageReference = document.StorageReference,
                Status = document.Status,
                Notes = document.Notes,
                ReceivedOn = document.ReceivedOn
            };

       
    }
}
