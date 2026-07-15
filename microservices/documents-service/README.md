# ClaimSphere Documents Service

This branch performs the brokered integration-events step after the first
Strangler Fig extraction.

## Module Objective

Branch `13-brokered-integration-events` teaches how an extracted microservice
can publish business events without calling the modular monolith during the user
request.

The objective is to replace the branch 12 HTTP relay:

```text
Documents Service -> HTTP relay -> Modular Monolith
```

with an asynchronous brokered flow:

```text
Documents Service -> Outbox -> Local broker -> Modular Monolith consumer
```

This demonstrates the production pattern before adding production
infrastructure. The demo still uses in-memory storage and a local file broker,
but the architecture is intentionally shaped so those adapters can later be
replaced by a database outbox and RabbitMQ, Azure Service Bus, Kafka, or another
real broker.

Before this branch:

```text
React Portal -> Gateway -> Modular Monolith -> Documents Module
```

After this branch:

```text
React Portal -> Gateway -> Documents Service
React Portal -> Gateway -> Modular Monolith for every other capability
Documents Service -> Outbox -> Local broker -> Modular Monolith consumer
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

This service intentionally uses in-memory document storage for the course demo.
Branch `13-brokered-integration-events` adds an in-memory outbox and a local
file broker so the reliability pattern is visible before introducing external
infrastructure.

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
  IntegrationEvents/IIntegrationEventPublisher
  IntegrationEvents/Contracts
  IntegrationEvents/Outbox

HealthCare.Claims.DocumentsService.Domain
  Entities/ClaimDocument
  Enums/ClaimDocumentStatus
  Enums/ClaimDocumentType

HealthCare.Claims.DocumentsService.Infrastructure
  Repositories/InMemoryClaimDocumentRepository
  IntegrationEvents/InMemoryOutboxStore
  IntegrationEvents/LocalFileIntegrationEventBroker
  IntegrationEvents/OutboxIntegrationEventPublisher
  IntegrationEvents/OutboxPublisherBackgroundService
```

The API project owns HTTP concerns only. Application owns CQRS commands,
queries, handlers, DTOs, and the repository port. Domain owns the business
model. Infrastructure owns the current in-memory repository and local broker
integration-event delivery.

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

## Cross-Service Events

The extracted service now publishes document lifecycle events through an outbox
and broker boundary:

```text
Documents Service
  -> command succeeds
  -> integration event saved to outbox
  -> background publisher writes to the repo-level .local-broker/document-events
  -> Modular Monolith background consumer reads broker message
  -> Modular Monolith in-process event bus
  -> Audit, Communications, Reporting
```

This is intentionally demo-friendly. It shows the same shape as a production
outbox plus broker flow without requiring RabbitMQ, Kafka, Docker, or a database
yet. If the modular monolith is offline, messages stay in the local broker
directory until the modular monolith consumer starts.

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

Restart the gateway after gateway code changes. The cross-service event test
posts JSON through the gateway, so the running gateway must include the latest
header-forwarding fix.

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

Open the extracted service Swagger UI:

```text
http://localhost:5240/swagger
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

## PowerShell Test Flow

Run each long-running process in its own terminal from the repository root.

### 1. Start The Modular Monolith

```powershell
dotnet run --project .\modular-monolith\src\Api\HealthCare.Claims.ModularMonolith.Api
```

Confirm it is reachable:

```powershell
Invoke-RestMethod http://localhost:5220/api/platform/modules |
    Format-Table name, description
```

### 2. Start The Documents Service

```powershell
dotnet run --project .\microservices\documents-service\src\HealthCare.Claims.DocumentsService.Api
```

Confirm it is reachable:

```powershell
Invoke-RestMethod http://localhost:5240/api/platform/modules |
    Format-Table name, description
```

Swagger is available at:

```text
http://localhost:5240/swagger
```

### 3. Start The Gateway

```powershell
dotnet run --project .\gateway\src\HealthCare.Claims.Gateway
```

Confirm `/api/documents` is owned by the extracted service:

```powershell
Invoke-RestMethod http://localhost:5230/api/gateway/routes |
    Select-Object -ExpandProperty routes |
    Format-Table pathPrefix, upstreamName, destinationBaseUrl, extracted
```

Expected gateway route:

```text
PathPrefix        : /api/documents
UpstreamName      : DocumentsService
DestinationBaseUrl: http://localhost:5240
Extracted         : True
```

### 4. Check Broker Diagnostics

Both diagnostics must show the same `queueDirectory`:

```powershell
$documentsOutbox = Invoke-RestMethod http://localhost:5240/api/outbox
$brokerStatus = Invoke-RestMethod http://localhost:5220/api/integration-events/documents/broker

$documentsOutbox.queueDirectory
$brokerStatus.queueDirectory
```

Expected path shape:

```text
...\HealthCare-Monolith\.local-broker\document-events
```

If the paths differ, stop and restart the Documents service and modular
monolith from the repository root.

### 5. Register A Document

Call the gateway route, not the modular monolith route. This proves the
Strangler gateway is sending document traffic to the extracted service.

```powershell
$newDocument = @{
    claimNumber = "CLM-202606270001"
    documentType = "DiagnosticReport"
    fileName = "diagnostics-clm-202606270001.pdf"
    storageReference = "local://claims/CLM-202606270001/diagnostics.pdf"
    notes = "Uploaded during brokered integration-event test."
} | ConvertTo-Json

$createdDocument = Invoke-RestMethod http://localhost:5230/api/documents `
    -Method Post `
    -ContentType "application/json" `
    -Body $newDocument

$createdDocument |
    Format-List id, claimNumber, documentType, fileName, status
```

Keep the returned `id`. Verification and rejection use the document id, not the
claim number.

### 6. Verify The Registered Event

The Documents service writes the event to the outbox, the background publisher
publishes it to the local broker, and the modular monolith consumer converts it
to an internal business event.

```powershell
Start-Sleep -Seconds 3

Invoke-RestMethod http://localhost:5240/api/outbox |
    Select-Object -ExpandProperty messages |
    Format-Table eventType, status, attempts, publishedOn

Invoke-RestMethod http://localhost:5220/api/integration-events/documents/broker |
    Format-List broker, queueDirectory, queued, processed, failed

Invoke-RestMethod "http://localhost:5220/api/audit?module=Documents" |
    Format-Table eventName, entityReference, summary
```

Expected checks:

```text
Outbox eventType : DocumentRegisteredIntegrationEvent
Outbox status    : Published
Broker queued    : 0
Broker failed    : 0
Audit eventName  : DocumentRegisteredEvent
```

### 7. Verify A Document

```powershell
$verification = @{
    notes = "Verified through brokered integration events."
} | ConvertTo-Json

$verifiedDocument = Invoke-RestMethod "http://localhost:5230/api/documents/$($createdDocument.id)/verify" `
    -Method Post `
    -ContentType "application/json" `
    -Body $verification

$verifiedDocument |
    Format-List id, documentType, fileName, status, notes
```

Check the cross-service effects:

```powershell
Start-Sleep -Seconds 3

Invoke-RestMethod http://localhost:5240/api/outbox |
    Select-Object -ExpandProperty messages |
    Format-Table eventType, status, attempts, publishedOn

Invoke-RestMethod "http://localhost:5220/api/audit?module=Documents" |
    Format-Table eventName, entityReference, summary

Invoke-RestMethod "http://localhost:5220/api/notifications?sourceEvent=DocumentVerifiedEvent" |
    Format-Table recipient, subject, sourceEvent

Invoke-RestMethod http://localhost:5220/api/reports/operations |
    Select-Object documentsRegistered, documentsVerified, documentsRejected
```

Expected result: audit contains `DocumentVerifiedEvent`, notifications contain
a `DocumentVerifiedEvent` notification, and the operations report increments
`documentsVerified`.

### 8. Reject A Document

Create a second document first, because a verified document should not be reused
for the rejection scenario.

```powershell
$rejectableDocumentRequest = @{
    claimNumber = "CLM-202606270001"
    documentType = "FinalBill"
    fileName = "final-bill-rejected-clm-202606270001.pdf"
    storageReference = "local://claims/CLM-202606270001/final-bill-rejected.pdf"
    notes = "Uploaded for rejection test."
} | ConvertTo-Json

$rejectableDocument = Invoke-RestMethod http://localhost:5230/api/documents `
    -Method Post `
    -ContentType "application/json" `
    -Body $rejectableDocumentRequest

$rejection = @{
    notes = "Uploaded document is not readable."
} | ConvertTo-Json

$rejectedDocument = Invoke-RestMethod "http://localhost:5230/api/documents/$($rejectableDocument.id)/reject" `
    -Method Post `
    -ContentType "application/json" `
    -Body $rejection

$rejectedDocument |
    Format-List id, documentType, fileName, status, notes
```

Check the cross-service effects:

```powershell
Start-Sleep -Seconds 3

Invoke-RestMethod http://localhost:5240/api/outbox |
    Select-Object -ExpandProperty messages |
    Format-Table eventType, status, attempts, publishedOn

Invoke-RestMethod "http://localhost:5220/api/audit?module=Documents" |
    Format-Table eventName, entityReference, summary

Invoke-RestMethod "http://localhost:5220/api/notifications?sourceEvent=DocumentRejectedEvent" |
    Format-Table recipient, subject, sourceEvent

Invoke-RestMethod http://localhost:5220/api/reports/operations |
    Select-Object documentsRegistered, documentsVerified, documentsRejected
```

Expected result: audit contains `DocumentRejectedEvent`, notifications contain
a `DocumentRejectedEvent` notification, and the operations report increments
`documentsRejected`.

### 9. What Not To Test For The Main Demo

Do not manually post to this endpoint for the branch 13 happy path:

```text
POST http://localhost:5220/api/integration-events/documents/registered
```

That endpoint is useful for testing the modular monolith receiver directly, but
it bypasses the brokered flow. For this branch, the main demo should be:

```text
Gateway -> Documents Service -> Outbox -> Local broker -> Modular Monolith consumer
```

## Troubleshooting

If Visual Studio stops in the Documents service with:

```text
Expected a supported JSON media type but got 'application/json, application/json'
```

an old gateway process is still forwarding duplicate `Content-Type` values.
Stop and restart the gateway, then rerun the verification request.

If the broker status stays at:

```text
queued    : 0
processed : 0
failed    : 0
```

check that both diagnostics show the same `queueDirectory`:

```powershell
(Invoke-RestMethod http://localhost:5240/api/outbox).queueDirectory
(Invoke-RestMethod http://localhost:5220/api/integration-events/documents/broker).queueDirectory
```

They should both point to:

```text
...\HealthCare-Monolith\.local-broker\document-events
```

## Teaching Point

This is the Strangler Fig pattern in its first real form:

```text
One route moved.
Frontend unchanged.
Most capabilities still in the modular monolith.
Gateway controls ownership.
Cross-service events preserve existing subscribers.
Outbox plus brokered delivery removes the request-time dependency.
```

This branch still avoids external infrastructure. The broker is a local file
queue under `.local-broker/document-events`, and the outbox is in memory because
the service storage is still in memory. A production follow-up would persist the
outbox transactionally with the service database and replace the local broker
adapter with RabbitMQ, Azure Service Bus, Kafka, or another real broker.
