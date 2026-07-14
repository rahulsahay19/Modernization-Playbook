namespace HealthCare.Claims.Modules.Reporting.Domain
{
    public sealed class OperationalProjection
    {
        public int ClaimsSubmitted { get; private set; }
        public int ClaimsApproved { get; private set; }
        public int ClaimsRejected { get; private set; }
        public int DocumentsRegistered { get; private set; }

        public int DocumentsVerified { get; private set; }

        public int DocumentsRejected { get; private set; }
        public int PaymentsSettled { get; private set; }

        public decimal SubmittedClaimAmount { get; private set; }
        public decimal ApprovedClaimAmount { get; private set; }
        public decimal SettledPaymentAmount { get; private set; }
        public DateTimeOffset LastUpdatedOn { get; private set; } = DateTimeOffset.UtcNow;

        public void RecordClaimSubmitted(decimal amount)
        {
            ClaimsSubmitted++;
            SubmittedClaimAmount += amount;
            Touch();
        }

        public void RecordClaimApproved(decimal amount)
        {
            ClaimsApproved++;
            ApprovedClaimAmount += amount;
            Touch();
        }

        public void RecordClaimRejected()
        {
            ClaimsRejected++;
            Touch();
        }
        public void RecordDocumentRegistered()
        {
            DocumentsRegistered++;
            Touch();
        }
        public void RecordDocumentVerified()
        {
            DocumentsVerified++;
            Touch();
        }
        public void RecordDocumentRejected()
        {
            DocumentsRejected++;
            Touch();
        }

        public void RecordPaymentSettled(decimal amount)
        {
            PaymentsSettled++;
            SettledPaymentAmount += amount;
            Touch();
        }
        private void Touch() => LastUpdatedOn = DateTimeOffset.UtcNow;
    }
}
