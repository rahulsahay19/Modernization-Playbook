# Healthcare Claims Monolith

This is the starting application for the modernization journey. It is a single deployable ASP.NET Core API that handles policies, members, providers, claims, documents, payments, notifications, and reporting in one process.

## Why This Baseline Is Intentionally Monolithic

- All capabilities share the same application and SQLite database.
- Workflow code crosses policy, member, provider, document, payment, and notification responsibilities.
- Reporting reads directly from every area.
- Documents and payments update claim state directly.
- Notifications are written synchronously inside business workflows.

These choices make the application easy to demo at first, but they also create the modernization pressure points we will use in later branches.

## Run The API

```powershell
dotnet run --project .\monolith\src\HealthCare.Claims.Monolith\HealthCare.Claims.Monolith.csproj
```

On startup, the application creates and seeds a local SQLite database file:

```text
healthcare-claims-legacy-complete.db
```

Open Swagger at:

```text
http://localhost:5213/swagger
```

Run this command from the repository root. Keep this terminal open while using
Swagger or the React portal.

## Run The React Frontend

The React application is maintained at the repository root so the same portal
can be used throughout the monolith, modular-monolith, and microservices stages:

```text
frontend/healthcare-claims-portal/
```

### First-Time Setup

Open a second terminal at the repository root:

```powershell
cd .\frontend\healthcare-claims-portal
npm install
npm run dev
```

Open the portal at:

```text
http://localhost:5173
```

Vite proxies frontend `/api/*` requests to the monolith at
`http://localhost:5213`. The recommended local workflow is therefore:

1. Terminal 1: start the ASP.NET Core monolith on port `5213`.
2. Terminal 2: start the React frontend on port `5173`.
3. Open `http://localhost:5173` for the operational portal.
4. Open `http://localhost:5213/swagger` when demonstrating API requests.
5. Stop each application with `Ctrl+C` when the walkthrough is complete.

In the Development environment, the API intentionally does not redirect HTTP
requests from port `5213` to HTTPS port `7200`. This keeps Vite proxy requests
same-origin from the browser's perspective and avoids local certificate and CORS
redirect failures. Production environments continue to use HTTPS redirection.

The API also permits direct development requests from:

```text
http://localhost:5173
http://127.0.0.1:5173
```

Do not mix `localhost` and `127.0.0.1` when troubleshooting cookies or browser
storage because browsers treat them as different origins.

API enums are serialized as readable strings. For example, claim status is
returned as `"PendingDocuments"` instead of its numeric enum value. This keeps
Swagger and frontend contracts understandable and stable.

### Demo Mode And Live Mode

The portal supports two modes:

- **Live mode:** The .NET API is running and the portal displays data from the
  shared SQLite monolith database.
- **Demo mode:** The API is unavailable and the portal automatically displays
  representative walkthrough data. A visible `Demo mode` indicator appears in
  the navigation.

Demo mode is useful for presenting the user experience without starting the
backend. Start the monolith and select the frontend refresh button to return to
live data.

### Using The Portal

The left navigation exposes the monolith's business capabilities:

- **Overview:** Operational metrics, claims distribution, shared platform load,
  and recent audit activity.
- **Claims:** Search the processing queue and select a claim to inspect member,
  provider, line-item, document, and payment details.
- **Members, Providers, Policies:** Review master and reference data.
- **Documents:** Review uploaded evidence and verification status.
- **Payments:** Review scheduled, settled, and failed payments.
- **Notifications:** Review simulated email and SMS messages.
- **Audit trail:** Review business actions recorded by the monolith.

The portal is primarily a walkthrough UI. Swagger remains the best interface for
demonstrating create, update, approval, rejection, verification, and settlement
commands in this branch.

### Pointing The Frontend To Another Backend

For a deployed environment, create
`frontend/healthcare-claims-portal/.env.local`:

```text
VITE_API_BASE_URL=https://claims-api.example.com
```

Restart the React development server after changing this value. During the
strangler-fig stages, this URL will point to the API gateway or BFF. The gateway
will decide whether each request is served by the monolith or an extracted
microservice, allowing the frontend to remain stable.

## Walkthrough Flow

1. Review seeded policies, members, providers, claims, documents, and reports.
2. Create or update policies, members, and providers.
3. Submit a new claim.
4. Upload and verify claim documents.
5. Approve or reject the claim.
6. Schedule and settle payment.
7. Inspect notifications, audit log, and operational reporting.

## Swagger API Samples

Use Swagger to execute the same flow from the browser:

```text
http://localhost:5213/swagger
```

### 1. View Insurance Policies

```http
GET /api/policies
```

Sample response:

```json
[
  {
    "policyNumber": "POL-1001",
    "insuranceProviderName": "Star Health and Allied Insurance Co. Ltd.",
    "planName": "Family Health Optima Insurance Plan",
    "annualLimit": 250000,
    "deductible": 5000,
    "isActive": true
  }
]
```

### 2. View Healthcare Providers

```http
GET /api/providers
```

Seeded examples include `Apollo Hospitals` and `Dr. Lal PathLabs`.

### 3. Submit A Claim

```http
POST /api/claims
Content-Type: application/json
```

```json
{
  "memberId": "MEM-2002",
  "providerId": "PRV-202",
  "serviceDate": "2026-06-12",
  "lines": [
    {
      "code": "IMG-300",
      "description": "MRI scan",
      "amount": 42000
    },
    {
      "code": "CONS-120",
      "description": "Specialist consultation",
      "amount": 6500
    }
  ]
}
```

The sample claim amount is interpreted as Indian rupees, for example `Rs. 48,500.00`.

### 4. Maintain Reference Data

The monolith also exposes write APIs for master/reference data:

```http
POST /api/policies
PUT /api/policies/{policyNumber}
POST /api/policies/{policyNumber}/deactivate
POST /api/members
PUT /api/members/{memberId}
POST /api/members/{memberId}/deactivate
POST /api/providers
PUT /api/providers/{providerId}
POST /api/providers/{providerId}/deactivate
```

These APIs are intentionally in the same deployable unit and shared database as claims processing.

### 5. Upload A Claim Document

```http
POST /api/documents
Content-Type: application/json
```

```json
{
  "claimNumber": "CLM-90001",
  "fileName": "discharge-summary.pdf",
  "documentType": "DischargeSummary"
}
```

The system stores metadata and a simulated storage path such as:

```text
/SimulatedDocumentStore/CLM-90001/discharge-summary.pdf
```

### 6. Verify A Claim Document

```http
POST /api/documents/DOC-70001/verify
Content-Type: application/json
```

```json
{
  "isVerified": true,
  "rejectionReason": null
}
```

### 7. Approve A Claim

```http
POST /api/claims/CLM-90001/approve
Content-Type: application/json
```

```json
{
  "approvedAmount": 16250
}
```

The notification message uses rupee formatting, for example `Rs. 16,250.00`.

### 8. Schedule And Settle Payment

```http
POST /api/payments/claims/CLM-90001/schedule
```

Copy the returned `paymentId`, then settle the payment:

```http
POST /api/payments/{paymentId}/settle
Content-Type: application/json
```

```json
{
  "settlementReference": "UTR-20260622-0001",
  "settledDate": "2026-06-22"
}
```

### 9. View Operational Report

```http
GET /api/reports/operations
```

Optional filters:

```http
GET /api/reports/operations?from=2026-06-01&to=2026-06-30
```

The report intentionally reads across policies, members, providers, claims, documents, payments, and notifications. This is useful for showing why reporting becomes a modernization concern.

### 10. View Audit Log

```http
GET /api/audit
```

The audit log is deliberately shared across all domains, another useful monolith modernization smell.

## Course Modernization Notes

This branch represents the legacy baseline. Later stages will discover bounded contexts, refactor toward a modular monolith, introduce module boundaries, and then extract selected capabilities using the strangler fig pattern.

The shared SQLite database is deliberately simple for local demos, but architecturally it represents the same coupling teams often have with SQL Server or Oracle in enterprise monoliths. Every module can read or update the same tables, which makes ownership, release isolation, and service extraction difficult.

## Monolith Pain Points To Observe

- Reference data, claims, documents, payments, notifications, reporting, and audit all ship together.
- Claim approval depends on policy, member, provider, document, notification, and audit behavior.
- Reporting reads across many tables instead of an isolated reporting model.
- Payment settlement updates payment state and claim state in the same database.
- Notifications and audit records are written synchronously inside business workflows.
- The shared database makes ownership unclear even though code now has controllers, services, and repositories.
