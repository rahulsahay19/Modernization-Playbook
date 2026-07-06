namespace HealthCare.Claims.Modules.Claims.Domain
{
    public sealed class Claim
    {
        public Claim(
        Guid id,
        string claimNumber,
        string policyNumber,
        string memberNumber,
        string providerCode,
        ClaimType claimType,
        DateOnly dateOfService,
        string diagnosis,
        string procedure,
        IReadOnlyCollection<ClaimLine> lines,
        ClaimStatus status,
        decimal? approvedAmount = null,
        string? decisionReason = null)
        {
            Id = id;
            ClaimNumber = claimNumber;
            PolicyNumber = policyNumber;
            MemberNumber = memberNumber;
            ProviderCode = providerCode;
            ClaimType = claimType;
            DateOfService = dateOfService;
            Diagnosis = diagnosis;
            Procedure = procedure;
            Lines = lines;
            Status = status;
            ApprovedAmount = approvedAmount;
            DecisionReason = decisionReason;
            SubmittedOn = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; }

        public string ClaimNumber { get; }

        public string PolicyNumber { get; }

        public string MemberNumber { get; }

        public string ProviderCode { get; }

        public ClaimType ClaimType { get; }

        public DateOnly DateOfService { get; }

        public string Diagnosis { get; }

        public string Procedure { get; }

        public IReadOnlyCollection<ClaimLine> Lines { get; }

        public ClaimStatus Status { get; private set; }

        public decimal TotalAmount => Lines.Sum(line => line.Amount);

        public decimal? ApprovedAmount { get; private set; }

        public string? DecisionReason { get; private set; }

        public DateTimeOffset SubmittedOn { get; }

        public void MarkUnderReview()
        {
            if(Status != ClaimStatus.Submitted)
            {
                throw new InvalidOperationException("Only submitted claims can be moved under review.");
            }
        }
        public void Approve(decimal approvedAmount, string reason)
        {
            if(Status is not ClaimStatus.Submitted and not ClaimStatus.UnderReview)
            {
                throw new InvalidOperationException("Only submitted or under-review claims can be approved.");
            }
            if(approvedAmount <=0 || approvedAmount > TotalAmount)
            {
                throw new InvalidOperationException("Approved amount must be greater than zero and within the claim amount.");
            }
            Status = ClaimStatus.Approved;
            ApprovedAmount = approvedAmount;
            DecisionReason = reason;
        }
        public void Reject(string reason)
        { 
            if(Status is not ClaimStatus.Submitted and not ClaimStatus.UnderReview)
            {
                throw new InvalidOperationException("Only submitted or under review claims can be rejected.");
            }
            Status = ClaimStatus.Rejected;
            ApprovedAmount = 0m;
            DecisionReason = reason;
        }
    }
}
