using HealthCare.Claims.Modules.Payments.Application;
using HealthCare.Claims.Modules.Payments.Domain;

namespace HealthCare.Claims.Modules.Payments.Infrastructure
{
    public sealed class InMemoryPaymentInstructionRepository : IPaymentInstructionRepository
    {
        private readonly List<PaymentInstruction> payments = [
            new(
                Guid.Parse("60000000-0000-0000-0000-000000000001"),
                "PAY-202606270001",
                "CLM-202606260001",
                "Aarav Sharma",
                "XXXXXX1234",
                "HDFC0001234",
                76000m,
                PaymentStatus.Scheduled,
                new DateOnly(2026, 6, 30),
                "Scheduled through NEFT batch."
                )
            ];

        private readonly object syncRoot = new();
        public void Add(PaymentInstruction payment)
        {
            lock(syncRoot)
            {
                payments.Add(payment);
            }
        }

        public PaymentInstruction? GetByClaimNumber(string claimNumber)
        {
            lock (syncRoot)
            {
                return payments.FirstOrDefault(payment =>
                string.Equals(payment.ClaimNumber, claimNumber, StringComparison.OrdinalIgnoreCase));
            }
        }

        public PaymentInstruction? GetById(Guid id)
        {
            lock (syncRoot)
            {
                return payments.FirstOrDefault(payment => payment.Id == id);
            }
        }

        public IReadOnlyCollection<PaymentInstruction> List()
        {
            lock (syncRoot)
            {
                return payments.ToArray();
            }
        }
    }
}
