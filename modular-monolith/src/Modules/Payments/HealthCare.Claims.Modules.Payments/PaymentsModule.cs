using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Payments.Application;
using HealthCare.Claims.Modules.Payments.Endpoints;
using HealthCare.Claims.Modules.Payments.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Payments
{
    public sealed class PaymentsModule : IModule
    {
        public string Name => "Payments";

        public string Description => "Approved-claim payment scheduling and settlement";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPaymentInstructionRepository, InMemoryPaymentInstructionRepository>();
            services.AddScoped<PaymentApplicationService>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            PaymentEndpoints.Map(endpoints);
        }
    }
}
