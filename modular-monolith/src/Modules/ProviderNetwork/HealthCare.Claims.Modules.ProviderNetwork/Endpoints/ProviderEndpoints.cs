using HealthCare.Claims.Modules.ProviderNetwork.Application;
using HealthCare.Claims.Modules.ProviderNetwork.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HealthCare.Claims.Modules.ProviderNetwork.Endpoints
{
    public static class ProviderEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/providers").WithTags("Provider Network");

            group.MapGet(
                "/",
                (ProviderApplicationService providers,
                string? city,
                string? speciality,
                bool? cashlessEnabled,
                ProviderNetworkTier? networkTier) =>
                Results.Ok(providers.List(city, speciality, cashlessEnabled, networkTier)));

            group.MapGet("/{id:guid}", (Guid id, ProviderApplicationService providers) =>
            {
                var provider = providers.GetById(id);
                return provider is null ? Results.NotFound() : Results.Ok(provider);
            });
        }        
    }
}
