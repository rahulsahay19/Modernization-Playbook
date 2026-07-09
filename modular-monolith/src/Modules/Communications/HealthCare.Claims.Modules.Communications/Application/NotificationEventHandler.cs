using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.Modules.Communications.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Claims.Modules.Communications.Application
{
    public sealed class NotificationEventHandler(INotificationRepository notifications) :
        IIntegrationEventHandler<ClaimSubmittedEvent>,
        IIntegrationEventHandler<ClaimApprovedEvent>,
        IIntegrationEventHandler<ClaimRejectedEvent>,
        IIntegrationEventHandler<DocumentVerifiedEvent>,
        IIntegrationEventHandler<PaymentSettledEvent>
    {
        public void Handle(ClaimSubmittedEvent integrationEvent) =>
        Add(
            $"member:{integrationEvent.MemberNumber}",
            NotificationChannel.Sms,
            "Claim submitted",
            $"Claim {integrationEvent.ClaimNumber} has been submitted for Rs {integrationEvent.TotalAmount}.",
            nameof(ClaimSubmittedEvent));

        public void Handle(ClaimApprovedEvent integrationEvent) =>
            Add(
                $"member:{integrationEvent.MemberNumber}",
                NotificationChannel.Email,
                "Claim approved",
                $"Claim {integrationEvent.ClaimNumber} has been approved for Rs {integrationEvent.ApprovedAmount}.",
                nameof(ClaimApprovedEvent));

        public void Handle(ClaimRejectedEvent integrationEvent) =>
            Add(
                $"member:{integrationEvent.MemberNumber}",
                NotificationChannel.Email,
                "Claim rejected",
                $"Claim {integrationEvent.ClaimNumber} was rejected. Reason: {integrationEvent.Reason}",
                nameof(ClaimRejectedEvent));

        public void Handle(DocumentVerifiedEvent integrationEvent) =>
            Add(
                $"claim:{integrationEvent.ClaimNumber}",
                NotificationChannel.Sms,
                "Document verified",
                $"{integrationEvent.DocumentType} document has been verified for claim {integrationEvent.ClaimNumber}.",
                nameof(DocumentVerifiedEvent));

        public void Handle(PaymentSettledEvent integrationEvent) =>
            Add(
                $"payee:{integrationEvent.PayeeName}",
                NotificationChannel.Email,
                "Payment settled",
                $"Payment {integrationEvent.PaymentNumber} for claim {integrationEvent.ClaimNumber} was settled for Rs {integrationEvent.Amount}.",
                nameof(PaymentSettledEvent));

        private void Add(
            string recipient,
            NotificationChannel channel,
            string subject,
            string body,
            string sourceEvent) =>
            notifications.Add(new NotificationMessage(
                Guid.NewGuid(),
                recipient,
                channel,
                subject,
                body,
                sourceEvent,
                NotificationStatus.Sent,
                DateTimeOffset.UtcNow));
    }
}
