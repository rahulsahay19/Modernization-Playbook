using HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData;
using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public sealed class ClaimApplicationService(
        IClaimRepository claims,
        IPolicyEligibilityReader policies,
        IMemberEligibilityReader members,
        IProviderNetworkReader providers)
    {
        public IReadOnlyCollection<ClaimSummary> List(ClaimStatus? status = null, string? memberNumber = null) 
        {
            var query = claims.List().AsEnumerable();

            if (status is not null)
            {
                query = query.Where(claim => claim.Status == status);
            }

            if(!string.IsNullOrWhiteSpace(memberNumber))
            {
                query = query.Where(claim => string.Equals(claim.MemberNumber, memberNumber, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderByDescending(claim => claim.SubmittedOn)
                .Select(ClaimSummary.FromClaim)
                .ToArray();
        }

        public ClaimDetails? GetById(Guid id)
        { 
            var claim = claims.GetById(id);
            return claim is null ? null : ClaimDetails.FromClaim(claim);
        }

        public ClaimDetails Submit(SubmitClaimRequest request)
        {
            ValidateSubmission(request);
            var claim = new Claim(
                Guid.NewGuid(),
                $"CLM-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
                request.PolicyNumber,
                request.MemberNumber,
                request.ProviderCode,
                request.ClaimType,
                request.DateOfService,
                request.Diagnosis,
                request.Procedure,
                request.Lines.Select(line => new ClaimLine(line.Description, line.Amount)).ToArray(),
                ClaimStatus.Submitted);

            claims.Add(claim);
            return ClaimDetails.FromClaim(claim);
        }

        public ClaimDetails? MarkUnderReview(Guid id)
        {
            var claim = claims.GetById(id);
            if(claim is null)
            {
                return null;
            }
            claim.MarkUnderReview();
            return ClaimDetails.FromClaim(claim);
        }

        public ClaimDetails? Approve(Guid id, ClaimDecisionRequest request)
        {
            var claim = claims.GetById(id);
            if (claim is null)
            {
                return null;
            }

            var approvedAmount = request.ApprovedAmount ?? claim.TotalAmount;
            claim.Approve(approvedAmount, request.Reason);

            return ClaimDetails.FromClaim(claim);
        }

        public ClaimDetails? Reject(Guid id, ClaimDecisionRequest request)
        {
            var claim = claims.GetById(id);
            if (claim is null)
            {
                return null;
            }
            
            claim.Reject(request.Reason);
            return ClaimDetails.FromClaim(claim);
        }

        private void ValidateSubmission(SubmitClaimRequest request)
        {
            if (request.Lines.Count == 0)
            {
                throw new InvalidOperationException("At least one claim line is required.");
            }

            if (request.Lines.Any(line => line.Amount <= 0))
            {
                throw new InvalidOperationException("Claim line amounts must be greater than zero.");
            }

            var policy = policies.GetByPolicyNumber(request.PolicyNumber)
                ?? throw new InvalidOperationException("Policy was not found.");

            if (!policy.IsActive)
            {
                throw new InvalidOperationException("Policy is not active for claim submission.");
            }

            if (request.DateOfService < policy.EffectiveFrom || request.DateOfService > policy.EffectiveTo)
            {
                throw new InvalidOperationException("Date of service is outside policy coverage dates.");
            }

            var totalAmount = request.Lines.Sum(line => line.Amount);
            if (totalAmount > policy.AvailableBalance)
            {
                throw new InvalidOperationException("Claim amount exceeds available policy balance.");
            }

            var member = members.GetByMemberNumber(request.MemberNumber)
                ?? throw new InvalidOperationException("Member was not found.");

            if (!member.isActive)
            {
                throw new InvalidOperationException("Member is not active for claim submission.");
            }

            if (!string.Equals(member.PolicyNumber, request.PolicyNumber, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Member does not belong to the submitted policy.");
            }

            var provider = providers.GetByProviderCode(request.ProviderCode)
                ?? throw new InvalidOperationException("Provider was not found.");

            if (!provider.isActive)
            {
                throw new InvalidOperationException("Provider is not active in the network.");
            }

            if (request.ClaimType == ClaimType.Cashless && !provider.CashlessEnabled)
            {
                throw new InvalidOperationException("Provider is not enabled for cashless claims.");
            }
        }
    }
}
