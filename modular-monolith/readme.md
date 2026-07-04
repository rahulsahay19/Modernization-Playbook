# Module 04 - Modular Monolith Foundation

This module creates the modular monolith foundation from scratch. The goal is
not to extract microservices yet. The goal is to create one deployable ASP.NET
Core application where business capabilities are represented as separate
modules.

## Target Structure

```text
modular-monolith/
  src/
    Api/
      HealthCare.Claims.ModularMonolith.Api/
    BuildingBlocks/
      HealthCare.Claims.ModularMonolith.BuildingBlocks/
    Modules/
      Audit/
      Claims/
      Communications/
      Documents/
      Membership/
      Payments/
      Policy/
      ProviderNetwork/
      Reporting/
```

## 1. Create The Modular Monolith Solution

Run these commands from the repository root.

```powershell
mkdir modular-monolith
cd modular-monolith
dotnet new sln -n HealthCare.Claims.ModularMonolith
mkdir src
```

## 2. Create The API Host

The API project is the composition root. It hosts the modular monolith and
loads all business modules.

```powershell
mkdir src\Api
dotnet new webapi -n HealthCare.Claims.ModularMonolith.Api -o src\Api\HealthCare.Claims.ModularMonolith.Api
dotnet sln add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj
```

Optional first run:

```powershell
dotnet run --project src\Api\HealthCare.Claims.ModularMonolith.Api
```

## 3. Create BuildingBlocks

BuildingBlocks contains shared abstractions such as the module contract. It
should not contain business workflow logic.

```powershell
mkdir src\BuildingBlocks
dotnet new classlib -n HealthCare.Claims.ModularMonolith.BuildingBlocks -o src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks
dotnet sln add src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
```

Add the BuildingBlocks reference to the API host:

```powershell
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
```

## 4. Add The Module Contract

Create this folder:

```powershell
mkdir src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\Modules
```

Create this file:

```text
src/BuildingBlocks/HealthCare.Claims.ModularMonolith.BuildingBlocks/Modules/IModule.cs
```

Add:

```csharp
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;

public interface IModule
{
    string Name { get; }

    string Description { get; }

    void AddServices(IServiceCollection services, IConfiguration configuration);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
```

## 5. Create All Module Projects

Each module is a separate class library. These projects represent business
capability boundaries inside one deployable application.

```powershell
mkdir src\Modules

mkdir src\Modules\Policy
dotnet new classlib -n HealthCare.Claims.Modules.Policy -o src\Modules\Policy\HealthCare.Claims.Modules.Policy
dotnet sln add src\Modules\Policy\HealthCare.Claims.Modules.Policy\HealthCare.Claims.Modules.Policy.csproj

mkdir src\Modules\Membership
dotnet new classlib -n HealthCare.Claims.Modules.Membership -o src\Modules\Membership\HealthCare.Claims.Modules.Membership
dotnet sln add src\Modules\Membership\HealthCare.Claims.Modules.Membership\HealthCare.Claims.Modules.Membership.csproj

mkdir src\Modules\ProviderNetwork
dotnet new classlib -n HealthCare.Claims.Modules.ProviderNetwork -o src\Modules\ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork
dotnet sln add src\Modules\ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork.csproj

mkdir src\Modules\Claims
dotnet new classlib -n HealthCare.Claims.Modules.Claims -o src\Modules\Claims\HealthCare.Claims.Modules.Claims
dotnet sln add src\Modules\Claims\HealthCare.Claims.Modules.Claims\HealthCare.Claims.Modules.Claims.csproj

mkdir src\Modules\Documents
dotnet new classlib -n HealthCare.Claims.Modules.Documents -o src\Modules\Documents\HealthCare.Claims.Modules.Documents
dotnet sln add src\Modules\Documents\HealthCare.Claims.Modules.Documents\HealthCare.Claims.Modules.Documents.csproj

mkdir src\Modules\Payments
dotnet new classlib -n HealthCare.Claims.Modules.Payments -o src\Modules\Payments\HealthCare.Claims.Modules.Payments
dotnet sln add src\Modules\Payments\HealthCare.Claims.Modules.Payments\HealthCare.Claims.Modules.Payments.csproj

mkdir src\Modules\Communications
dotnet new classlib -n HealthCare.Claims.Modules.Communications -o src\Modules\Communications\HealthCare.Claims.Modules.Communications
dotnet sln add src\Modules\Communications\HealthCare.Claims.Modules.Communications\HealthCare.Claims.Modules.Communications.csproj

mkdir src\Modules\Audit
dotnet new classlib -n HealthCare.Claims.Modules.Audit -o src\Modules\Audit\HealthCare.Claims.Modules.Audit
dotnet sln add src\Modules\Audit\HealthCare.Claims.Modules.Audit\HealthCare.Claims.Modules.Audit.csproj

mkdir src\Modules\Reporting
dotnet new classlib -n HealthCare.Claims.Modules.Reporting -o src\Modules\Reporting\HealthCare.Claims.Modules.Reporting
dotnet sln add src\Modules\Reporting\HealthCare.Claims.Modules.Reporting\HealthCare.Claims.Modules.Reporting.csproj
```

## 6. Reference BuildingBlocks From Every Module

Each module implements `IModule`, so every module references BuildingBlocks.

```powershell
dotnet add src\Modules\Policy\HealthCare.Claims.Modules.Policy\HealthCare.Claims.Modules.Policy.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Membership\HealthCare.Claims.Modules.Membership\HealthCare.Claims.Modules.Membership.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Claims\HealthCare.Claims.Modules.Claims\HealthCare.Claims.Modules.Claims.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Documents\HealthCare.Claims.Modules.Documents\HealthCare.Claims.Modules.Documents.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Payments\HealthCare.Claims.Modules.Payments\HealthCare.Claims.Modules.Payments.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Communications\HealthCare.Claims.Modules.Communications\HealthCare.Claims.Modules.Communications.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Audit\HealthCare.Claims.Modules.Audit\HealthCare.Claims.Modules.Audit.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
dotnet add src\Modules\Reporting\HealthCare.Claims.Modules.Reporting\HealthCare.Claims.Modules.Reporting.csproj reference src\BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks\HealthCare.Claims.ModularMonolith.BuildingBlocks.csproj
```

## 7. Reference All Modules From The API Host

The API project is the composition root, so it references every module and
loads them at startup.

```powershell
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Policy\HealthCare.Claims.Modules.Policy\HealthCare.Claims.Modules.Policy.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Membership\HealthCare.Claims.Modules.Membership\HealthCare.Claims.Modules.Membership.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork\HealthCare.Claims.Modules.ProviderNetwork.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Claims\HealthCare.Claims.Modules.Claims\HealthCare.Claims.Modules.Claims.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Documents\HealthCare.Claims.Modules.Documents\HealthCare.Claims.Modules.Documents.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Payments\HealthCare.Claims.Modules.Payments\HealthCare.Claims.Modules.Payments.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Communications\HealthCare.Claims.Modules.Communications\HealthCare.Claims.Modules.Communications.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Audit\HealthCare.Claims.Modules.Audit\HealthCare.Claims.Modules.Audit.csproj
dotnet add src\Api\HealthCare.Claims.ModularMonolith.Api\HealthCare.Claims.ModularMonolith.Api.csproj reference src\Modules\Reporting\HealthCare.Claims.Modules.Reporting\HealthCare.Claims.Modules.Reporting.csproj
```

## 8. Add Initial Module Classes

In each module project, create a module class that implements `IModule`.

Example:

```text
src/Modules/Communications/HealthCare.Claims.Modules.Communications/CommunicationsModule.cs
```

```csharp
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Claims.Modules.Communications;

public sealed class CommunicationsModule : IModule
{
    public string Name => "Communications";

    public string Description => "Email and SMS notification delivery.";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
```

Repeat this pattern for:

```text
AuditModule
ClaimsModule
DocumentsModule
MembershipModule
PaymentsModule
PolicyModule
ProviderNetworkModule
ReportingModule
```

## 9. Wire Modules In API Program.cs

Update:

```text
src/Api/HealthCare.Claims.ModularMonolith.Api/Program.cs
```

Use:

```csharp
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Modules;
using HealthCare.Claims.Modules.Audit;
using HealthCare.Claims.Modules.Claims;
using HealthCare.Claims.Modules.Communications;
using HealthCare.Claims.Modules.Documents;
using HealthCare.Claims.Modules.Membership;
using HealthCare.Claims.Modules.Payments;
using HealthCare.Claims.Modules.Policy;
using HealthCare.Claims.Modules.ProviderNetwork;
using HealthCare.Claims.Modules.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

IModule[] modules =
[
    new ClaimsModule(),
    new PolicyModule(),
    new MembershipModule(),
    new ProviderNetworkModule(),
    new DocumentsModule(),
    new PaymentsModule(),
    new CommunicationsModule(),
    new AuditModule(),
    new ReportingModule()
];

foreach (var module in modules)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/api/platform/modules", () =>
    Results.Ok(modules.Select(module => new
    {
        module.Name,
        module.Description
    })));

foreach (var module in modules)
{
    module.MapEndpoints(app);
}

app.Run();
```

## 10. Build And Run

Build the solution:

```powershell
dotnet build
```

Run the API host:

```powershell
dotnet run --project src\Api\HealthCare.Claims.ModularMonolith.Api
```

Call the platform endpoint:

```text
GET /api/platform/modules
```

Example local URL:

```text
http://localhost:5220/api/platform/modules
```

This should return the list of loaded modules.

## What This Module Proves

At this point, business endpoints such as `/api/claims` and `/api/policies`
may still return `404` because the module endpoint methods are empty.

That is expected.

This module proves the foundation:

- the API host can load modules;
- every module follows the same `IModule` contract;
- module metadata can be exposed from the platform endpoint;
- each module can later register its own services;
- each module can later map its own endpoints;
- the system is still one deployment, but business capability boundaries are now visible in code.

## Architecture Rule

The API host can reference all modules because it composes the application.

Each module can reference BuildingBlocks.

Business modules should not directly reference each other.

Use this direction:

```text
Api -> Modules
Api -> BuildingBlocks
Modules -> BuildingBlocks
```

Avoid this direction:

```text
Claims -> Policy
Documents -> Claims
Payments -> Claims
```

When modules need to collaborate, introduce explicit contracts or events.
