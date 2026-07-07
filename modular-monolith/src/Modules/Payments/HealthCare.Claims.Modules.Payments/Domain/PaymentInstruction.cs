namespace HealthCare.Claims.Modules.Payments.Domain
{
    public sealed class PaymentInstruction
    {
        public PaymentInstruction(
            Guid id,
            string paymentNumber,
            string claimNumber,
            string payeeName,
            string bankAccountMasked,
            string ifscCode,
            decimal amount,
            PaymentStatus status,
            DateOnly? scheduledOn = null,
            string? remarks = null)
        {
            Id = id;
            PaymentNumber = paymentNumber;
            ClaimNumber = claimNumber;
            PayeeName = payeeName;
            BankAccountMasked = bankAccountMasked;
            IfscCode = ifscCode;
            Amount = amount;
            Status = status;
            ScheduledOn = scheduledOn;
            Remarks = remarks;
            CreatedOn = DateTimeOffset.UtcNow;
        }
        public Guid Id { get; }

        public string PaymentNumber { get; }

        public string ClaimNumber { get; }

        public string PayeeName { get; }

        public string BankAccountMasked { get; }

        public string IfscCode { get; }

        public decimal Amount { get; }

        public PaymentStatus Status { get; private set; }

        public DateOnly? ScheduledOn { get; private set; }

        public string? Remarks { get; private set; }

        public DateTimeOffset CreatedOn { get; }

        public void Schedule(DateOnly scheduledOn, string? remarks)
        {
            if(Status !=PaymentStatus.Pending)
            {
                throw new InvalidOperationException("Only pending payment instructions can be scheduled.");
            }
            Status = PaymentStatus.Scheduled;
            ScheduledOn = scheduledOn;
            Remarks = remarks;
        }

        public void Settle(string? remarks)
        {
            if(Status is not PaymentStatus.Pending and not PaymentStatus.Scheduled)
            {
                throw new InvalidOperationException("Only pending or scheduled instructions can be settled.");
            }
            Status = PaymentStatus.Settled;
            Remarks = remarks;
        }

        public void MarkFailed(string remarks)
        {
            if(Status == PaymentStatus.Settled)
            {
                throw new InvalidOperationException("Settled payment instructions can't marked as failed.");
            }
            Status = PaymentStatus.Failed;
            Remarks = remarks;
        }
    }
   
}
