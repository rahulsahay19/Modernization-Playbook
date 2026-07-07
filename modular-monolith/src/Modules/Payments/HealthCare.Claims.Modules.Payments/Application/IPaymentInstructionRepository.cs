using HealthCare.Claims.Modules.Payments.Domain;

namespace HealthCare.Claims.Modules.Payments.Application
{
    public interface IPaymentInstructionRepository
    {
        IReadOnlyCollection<PaymentInstruction> List();
        PaymentInstruction? GetById(Guid id);
        PaymentInstruction? GetByClaimNumber(string claimNumber);
        void Add(PaymentInstruction payment);
    }
}
