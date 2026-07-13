# ClaimSphere Strangler Gateway

This branch performs the first Strangler Fig extraction.

```text
gateway/
  HealthCare.Claims.Gateway.slnx
  src/
    HealthCare.Claims.Gateway/
```

The important architectural change is that the React portal still calls a stable
gateway URL while the gateway moves one capability to an extracted service.

```text
React portal -> API Gateway -> Modular monolith
```

This branch moves the first capability behind the same gateway route:

```text
React portal -> API Gateway -> Modular monolith
                           -> Extracted Documents service
```

## Run

Start the modular monolith for every capability except Documents:

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

Start the frontend through the gateway:

```powershell
cd .\frontend\healthcare-claims-portal
npm run dev:gateway
```

Open:

```text
http://localhost:5173
```

## Verify Gateway Routing

The gateway runs on:

```text
http://localhost:5230
```

Inspect the route table:

```powershell
Invoke-RestMethod http://localhost:5230/api/gateway/routes
```

Check backend reachability:

```powershell
Invoke-RestMethod http://localhost:5230/api/gateway/health
```

Call existing APIs through the gateway:

```powershell
Invoke-RestMethod http://localhost:5230/api/platform/modules
Invoke-RestMethod http://localhost:5230/api/claims
Invoke-RestMethod http://localhost:5230/api/documents
Invoke-RestMethod http://localhost:5230/api/reports/operations
```

Responses include these headers:

```text
X-Strangler-Route
X-Upstream-Service
```

Those headers make the course demo visible: the frontend route stays stable
while the upstream owner can change.

## Screen Test From The React Portal

Use this as the branch `11-extract-documents-service` demo script.

1. Start the modular monolith.

   ```powershell
   dotnet run --project .\modular-monolith\src\Api\HealthCare.Claims.ModularMonolith.Api
   ```

2. Start the extracted Documents service.

   ```powershell
   dotnet run --project .\microservices\documents-service\src\HealthCare.Claims.DocumentsService.Api
   ```

3. Start the gateway.

   ```powershell
   dotnet run --project .\gateway\src\HealthCare.Claims.Gateway
   ```

4. Start the React portal in gateway mode.

   ```powershell
   cd .\frontend\healthcare-claims-portal
   npm run dev:gateway
   ```

5. Open the portal.

   ```text
   http://localhost:5173
   ```

6. Confirm the sidebar connection message.

   ```text
   Strangler gateway connected
   ```

   This proves the browser is still using the same React application, while
   `/api/*` calls now flow through the gateway.

7. Click through the portal screens:

   - **Overview**
   - **Claims**
   - **Members**
   - **Providers**
   - **Policies**
   - **Documents**
   - **Payments**
   - **Notifications**
   - **Audit trail**

   Each screen should continue to load data. **Documents** is served by the
   extracted service; the other screens still come from the modular monolith.

8. Open **Documents** and confirm document rows load.

   That is the main extraction proof from the UI: the frontend did not change,
   but `/api/documents` is now owned by a separate process.

9. On **Claims**, select different claim rows and use the search box.

   The claim detail panel should update exactly as it did before the gateway was
   introduced.

10. Open gateway diagnostics directly:

   ```text
   http://localhost:5230/api/gateway/routes
   http://localhost:5230/api/gateway/health
   ```

11. Call API routes through the gateway:

   ```text
   http://localhost:5230/api/claims
   http://localhost:5230/api/documents
   http://localhost:5230/api/reports/operations
   ```

Expected request path:

```text
React Portal -> Vite proxy -> Strangler Gateway -> Documents Service
React Portal -> Vite proxy -> Strangler Gateway -> Modular Monolith for everything else
```

The requested and settled amount cards can show `Rs 0` after a restart because
Reporting is still an in-memory event projection in this demo branch. Trigger new
claim, document, or payment events if you want those event-derived numbers to
increase during recording.

## Route Ownership

The Documents route now points to the extracted service on port `5240`.
All other routes still point to the modular monolith on port `5220`.

That is intentional. This branch moves one capability while keeping the frontend
and all other backend capabilities stable.

| Route prefix | Current owner | Extraction status |
| --- | --- | --- |
| `/api/claims` | Modular monolith | Not extracted |
| `/api/documents` | Documents service | Extracted |
| `/api/payments` | Modular monolith | Not extracted |
| `/api/policies` | Modular monolith | Not extracted |
| `/api/members` | Modular monolith | Not extracted |
| `/api/providers` | Modular monolith | Not extracted |
| `/api/notifications` | Modular monolith | Not extracted |
| `/api/audit` | Modular monolith | Not extracted |
| `/api/reports` | Modular monolith | Not extracted |

## Teaching Point

This branch changes service ownership for exactly one route.

That is the safer modernization move:

1. Put a gateway in front of the existing modular monolith.
2. Keep frontend URLs stable.
3. Add route observability.
4. Extract Documents as a separate service.
5. Change only the gateway route target for `/api/documents`.

The next useful branch can extract another capability or add cross-process
messaging:

```text
12-extract-payments-service
```
