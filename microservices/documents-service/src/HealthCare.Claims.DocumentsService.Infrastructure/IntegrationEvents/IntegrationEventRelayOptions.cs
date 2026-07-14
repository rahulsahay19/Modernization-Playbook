using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class IntegrationEventRelayOptions
    {
        public string ModularMonolithBaseUrl { get; set; } = "http://localhost:5220";
    }
}
