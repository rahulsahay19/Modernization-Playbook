using HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.Modules.Payments.Domain;

namespace HealthCare.Claims.Modules.Payments.Application
{
    public sealed class PaymentApplicationService(
        IPaymentInstructionRepository payments,
        IClaimReferenceReader claims,
        IEventBus eventBus)
    {
        public IReadOnlyCollection<PaymentInstructionSummary> List(string? claimNumber = null, PaymentStatus? status = null)
        {
            var query = payments.List().AsEnumerable();
            if(!string.IsNullOrWhiteSpace(claimNumber))
            {
                query = query.Where(payment =>
                        string.Equals(payment.ClaimNumber, claimNumber, StringComparison.OrdinalIgnoreCase));
            }
            if(status is not null)
            {
                query = query.Where(payment => payment.Status == status);
            }
            return query
                  .OrderByDescending(payment => payment.CreatedOn)
                  .Select(PaymentInstructionSummary.FromPayment)
                  .ToArray();
        }
        public PaymentInstructionSummary? GetById(Guid id)
        {
            var payment = payments.GetById(id);
            return payment is null ? null : PaymentInstructionSummary.FromPayment(payment);
        }

        public PaymentInstructionSummary Create(CreatePaymentInstructionRequest request)
        {
            var claim = claims.GetByClaimNumber(request.ClaimNumber)
                        ?? throw new InvalidOperationException("Claim was not found.");
            if (!string.Equals(claim.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Payment can be created only for approved claims.");
            }
            if(claim.ApprovedAmount is null or <=0)
            {
                throw new InvalidOperationException("Approved claim amount is missing.");
            }
            if(payments.GetByClaimNumber(request.ClaimNumber) is not null)
            {
                throw new InvalidOperationException("Payment instruction already exists for this claim");
            }

            var payment = new PaymentInstruction(
            Guid.NewGuid(),
            $"PAY-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
            request.ClaimNumber,
            request.PayeeName,
            request.BankAccountMasked,
            request.IfscCode,
            claim.ApprovedAmount.Value,
            PaymentStatus.Pending);

            payments.Add(payment);
            return PaymentInstructionSummary.FromPayment(payment);
        }
        public PaymentInstructionSummary? Schedule(Guid id, SchedulePaymentRequest request)
        {
            var payment = payments.GetById(id);

            if (payment is null)
            {
                return null;
            }

            payment.Schedule(request.ScheduledOn, request.Remarks);

            return PaymentInstructionSummary.FromPayment(payment);
        }

        public PaymentInstructionSummary? Settle(Guid id, PaymentRemarksRequest request)
        {
            var payment = payments.GetById(id);

            if (payment is null)
            {
                return null;
            }

            payment.Settle(request.Remarks);
            eventBus.Publish(new PaymentSettledEvent(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                payment.PaymentNumber,
                payment.ClaimNumber,
                payment.PayeeName,
                payment.Amount));
            return PaymentInstructionSummary.FromPayment(payment);
        }

        public PaymentInstructionSummary? MarkFailed(Guid id, PaymentRemarksRequest request)
        {
            var payment = payments.GetById(id);

            if (payment is null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Remarks))
            {
                throw new InvalidOperationException("Failure remarks are required.");
            }

            payment.MarkFailed(request.Remarks);

            return PaymentInstructionSummary.FromPayment(payment);
        }
    }
}
