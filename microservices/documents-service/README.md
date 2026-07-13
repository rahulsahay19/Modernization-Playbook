# ClaimSphere Documents Service

This branch performs the first real Strangler Fig extraction.

Before this branch:

```text
React Portal -> Gateway -> Modular Monolith -> Documents Module
```

After this branch:

```text
React Portal -> Gateway -> Documents Service
React Portal -> Gateway -> Modular Monolith for every other capability
```

The frontend still calls:

```text
/api/documents
```

Only the gateway route target changes.

## Solution Structure

```text
microservices/documents-service/
  HealthCare.Claims.DocumentsService.slnx
  src/
    HealthCare.Claims.DocumentsService.Api/
      Program.cs
      Endpoints/
    HealthCare.Claims.DocumentsService.Application/
      Abstractions/
      Commands/
      DTOs/
      Handlers/
      Queries/
    HealthCare.Claims.DocumentsService.Domain/
      Entities/
      Enums/
    HealthCare.Claims.DocumentsService.Infrastructure/
      Repositories/
```

This service intentionally uses in-memory storage for the course demo. The
production lesson comes later: service-owned persistence, outbox messaging, and
contract versioning.

## Internal Architecture

The extracted service uses a full clean architecture project split:

```text
HealthCare.Claims.DocumentsService.Api
  Program.cs
  Endpoints/

HealthCare.Claims.DocumentsService.Application
  Abstractions/IClaimDocumentRepository
  Abstractions/ICommandHandler
  Abstractions/IQueryHandler
  Commands/Documents/RegisterDocumentCommand
  Commands/Documents/VerifyDocumentCommand
  Commands/Documents/RejectDocumentCommand
  Queries/Documents/ListDocumentsQuery
  Queries/Documents/GetDocumentQuery
  Handlers/Documents/ListDocumentsQueryHandler
  Handlers/Documents/GetDocumentQueryHandler
  Handlers/Documents/RegisterDocumentCommandHandler
  Handlers/Documents/VerifyDocumentCommandHandler
  Handlers/Documents/RejectDocumentCommandHandler
  DTOs/DocumentResponse
  DTOs/DocumentCommandResult

HealthCare.Claims.DocumentsService.Domain
  Entities/ClaimDocument
  Enums/ClaimDocumentStatus
  Enums/ClaimDocumentType

HealthCare.Claims.DocumentsService.Infrastructure
  Repositories/InMemoryClaimDocumentRepository
```

The API project owns HTTP concerns only. Application owns CQRS commands,
queries, handlers, DTOs, and the repository port. Domain owns the business
model. Infrastructure owns the current in-memory repository implementation.

This service uses CQRS without MediatR. Endpoints inject small command/query
handler interfaces directly:

```text
IQueryHandler<TQuery, TResult>
ICommandHandler<TCommand, TResult>
```

The reads are query handlers:

```text
ListDocumentsQueryHandler
GetDocumentQueryHandler
```

The writes are command handlers:

```text
RegisterDocumentCommandHandler
VerifyDocumentCommandHandler
RejectDocumentCommandHandler
```

## Run

Start the modular monolith for all non-documents capabilities:

```powershell
dotnet run --project .\modular-monolith\src\Api\HealthCare.Claims.ModularMonolith.Api
```

Start the extracted Documents service:

```powershell
dotnet run --project .\microservices\documents-service\src\HealthCare.Claims.DocumentsService.Api
```

Start the gateway:

```powershell
dotnet run --project .\gateway\src\HealthCare.Claims.Gateway
```

Start the React portal:

```powershell
cd .\frontend\healthcare-claims-portal
npm run dev:gateway
```

Open:

```text
http://localhost:5173
```

## Verify Extraction

Inspect the gateway route table:

```powershell
Invoke-RestMethod http://localhost:5230/api/gateway/routes
```

The `/api/documents` route should show:

```text
UpstreamName: DocumentsService
DestinationBaseUrl: http://localhost:5240
Extracted: true
```

Check gateway health:

```powershell
Invoke-RestMethod http://localhost:5230/api/gateway/health
```

Call Documents through the gateway:

```powershell
Invoke-RestMethod http://localhost:5230/api/documents
```

Call the service directly:

```powershell
Invoke-RestMethod http://localhost:5240/api/documents
```

The gateway response includes:

```text
X-Strangler-Route: documents
X-Upstream-Service: DocumentsService
```

## Screen Test

1. Open `http://localhost:5173`.
2. Confirm the sidebar says **Strangler gateway connected**.
3. Open **Documents**.
4. Verify document rows load.
5. Open `http://localhost:5230/api/gateway/routes`.
6. Confirm only `/api/documents` is extracted.
7. Open **Claims**, **Payments**, and **Reports**.
8. Confirm those screens still work through the modular monolith.

## Teaching Point

This is the Strangler Fig pattern in its first real form:

```text
One route moved.
Frontend unchanged.
Most capabilities still in the modular monolith.
Gateway controls ownership.
```

This branch does not introduce a message broker yet. Document verification
events are local to the extracted service in this demo. A later branch can add
brokered integration events so Reporting, Audit, and Communications receive
document events across process boundaries.
