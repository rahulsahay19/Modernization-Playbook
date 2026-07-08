using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.Modules.Audit.Domain;

namespace HealthCare.Claims.Modules.Audit.Application
{
    public sealed class AuditEventHandler(IAuditEntryRepository auditEntries) :
        IIntegrationEventHandler<ClaimSubmittedEvent>,
        IIntegrationEventHandler<ClaimApprovedEvent>,
        IIntegrationEventHandler<ClaimRejectedEvent>,
        IIntegrationEventHandler<DocumentVerifiedEvent>,
        IIntegrationEventHandler<PaymentSettledEvent>
    {
        void IIntegrationEventHandler<ClaimSubmittedEvent>.Handle(ClaimSubmittedEvent integrationEvent)
        {
            Add("Claims",
                nameof(ClaimSubmittedEvent),
                integrationEvent.ClaimNumber,
                $"Claim submitted for member {integrationEvent.MemberNumber} with amount Rs {integrationEvent.TotalAmount}.");
        }
        public void Handle(ClaimApprovedEvent integrationEvent) =>
       Add(
           "Claims",
           nameof(ClaimApprovedEvent),
           integrationEvent.ClaimNumber,
           $"Claim approved for Rs {integrationEvent.ApprovedAmount}. Reason: {integrationEvent.Reason}");

        public void Handle(ClaimRejectedEvent integrationEvent) =>
            Add(
                "Claims",
                nameof(ClaimRejectedEvent),
                integrationEvent.ClaimNumber,
                $"Claim rejected. Reason: {integrationEvent.Reason}");

        public void Handle(DocumentVerifiedEvent integrationEvent) =>
            Add(
                "Documents",
                nameof(DocumentVerifiedEvent),
                integrationEvent.ClaimNumber,
                $"{integrationEvent.DocumentType} document {integrationEvent.FileName} verified.");

        public void Handle(PaymentSettledEvent integrationEvent) =>
            Add(
                "Payments",
                nameof(PaymentSettledEvent),
                integrationEvent.PaymentNumber,
                $"Payment settled for claim {integrationEvent.ClaimNumber}, payee {integrationEvent.PayeeName}, amount Rs {integrationEvent.Amount}.");



        void IIntegrationEventHandler<ClaimRejectedEvent>.Handle(ClaimRejectedEvent integrationEvent)
        {
            throw new NotImplementedException();
        }

        private void Add(string module, string eventName, string entityReference, string summary) =>
            auditEntries.Add(new AuditEntry(Guid.NewGuid(), module, eventName, entityReference, summary, DateTimeOffset.UtcNow));
    }
}
