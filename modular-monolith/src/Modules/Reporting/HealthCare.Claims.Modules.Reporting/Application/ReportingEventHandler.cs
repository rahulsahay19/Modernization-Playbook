using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.Modules.Reporting.Domain;

namespace HealthCare.Claims.Modules.Reporting.Application;

public sealed class ReportingEventHandler(IReportingProjectionRepository projections) :
    IIntegrationEventHandler<ClaimSubmittedEvent>,
    IIntegrationEventHandler<ClaimApprovedEvent>,
    IIntegrationEventHandler<ClaimRejectedEvent>,
    IIntegrationEventHandler<DocumentVerifiedEvent>,
    IIntegrationEventHandler<PaymentSettledEvent>
{
    public void Handle(ClaimSubmittedEvent integrationEvent)
    {
        var projection = projections.GetProjection();
        projection.RecordClaimSubmitted(integrationEvent.TotalAmount);
        AddRecent(nameof(ClaimSubmittedEvent), integrationEvent.ClaimNumber, $"Submitted claim for Rs {integrationEvent.TotalAmount}.", integrationEvent.OccurredOn);
    }

    public void Handle(ClaimApprovedEvent integrationEvent)
    {
        var projection = projections.GetProjection();
        projection.RecordClaimApproved(integrationEvent.ApprovedAmount);
        AddRecent(nameof(ClaimApprovedEvent), integrationEvent.ClaimNumber, $"Approved claim for Rs {integrationEvent.ApprovedAmount}.", integrationEvent.OccurredOn);
    }

    public void Handle(ClaimRejectedEvent integrationEvent)
    {
        var projection = projections.GetProjection();
        projection.RecordClaimRejected();
        AddRecent(nameof(ClaimRejectedEvent), integrationEvent.ClaimNumber, $"Rejected claim. Reason: {integrationEvent.Reason}", integrationEvent.OccurredOn);
    }

    public void Handle(DocumentVerifiedEvent integrationEvent)
    {
        var projection = projections.GetProjection();
        projection.RecordDocumentVerified();
        AddRecent(nameof(DocumentVerifiedEvent), integrationEvent.ClaimNumber, $"Verified {integrationEvent.DocumentType} document.", integrationEvent.OccurredOn);
    }

    public void Handle(PaymentSettledEvent integrationEvent)
    {
        var projection = projections.GetProjection();
        projection.RecordPaymentSettled(integrationEvent.Amount);
        AddRecent(nameof(PaymentSettledEvent), integrationEvent.PaymentNumber, $"Settled payment for Rs {integrationEvent.Amount}.", integrationEvent.OccurredOn);
    }

    private void AddRecent(string eventName, string entityReference, string summary, DateTimeOffset occurredOn) =>
        projections.AddRecentEvent(new RecentBusinessEvent(eventName, entityReference, summary, occurredOn));
}
