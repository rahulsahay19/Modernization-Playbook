# Healthcare Claims Portal

The React frontend is intentionally outside every backend implementation:

```text
frontend/healthcare-claims-portal
monolith/
modular-monolith/       # added later in the course
gateway/                # added during the strangler stage
microservices/          # added later in the course
```

This lets the course modernize the backend without rewriting the user experience.
The UI calls stable `/api/*` contracts through `src/api/claimsApi.ts`.

## Run locally

Start the monolith:

```powershell
dotnet run --project monolith/src/HealthCare.Claims.Monolith
```

Start React in another terminal:

```powershell
cd frontend/healthcare-claims-portal
npm install
npm run dev:monolith
```

Open `http://localhost:5173`. Vite proxies `/api` requests to
`http://localhost:5213`.

The monolith does not force HTTP-to-HTTPS redirects in Development, so proxied
API calls remain on the Vite origin and do not trigger browser CORS errors.

When the API is not running, the portal uses representative walkthrough data and
shows a visible **Demo mode** indicator.

## Switching backends

For the modular monolith, start:

```powershell
dotnet run --project modular-monolith/src/Api/HealthCare.Claims.ModularMonolith.Api
```

Then start React with:

```powershell
cd frontend/healthcare-claims-portal
npm run dev:modular
```

Use `http://localhost:5213` for the original monolith and
`http://localhost:5220` for the modular monolith.

When connected to the modular monolith, the sidebar should show:

```text
Modular monolith connected
```

If it still shows demo mode, stop and restart React with `npm run dev:modular`.
Vite reads proxy settings only when the dev server starts.

For the Strangler gateway stage, start the modular monolith first, then start:

```powershell
dotnet run --project gateway/src/HealthCare.Claims.Gateway
```

Then start React with:

```powershell
cd frontend/healthcare-claims-portal
npm run dev:gateway
```

The portal still calls `/api/*`, but Vite sends those calls to the gateway on
`http://localhost:5230`. The gateway then routes each capability to its current
backend owner.

When connected through the gateway, the sidebar should show:

```text
Strangler gateway connected
```

Use this quick screen test:

1. Open `http://localhost:5173`.
2. Confirm the sidebar says **Strangler gateway connected**.
3. Open **Overview**, **Claims**, **Members**, **Providers**, **Policies**,
   **Documents**, **Payments**, **Notifications**, and **Audit trail**.
4. On **Claims**, select different rows and verify the detail panel updates.
5. Open `http://localhost:5230/api/gateway/routes` to show the gateway route
   table.
6. Open `http://localhost:5230/api/gateway/health` to show upstream reachability.

The request flow for this branch is:

```text
React Portal -> Vite proxy -> Strangler Gateway -> Modular Monolith
```

You can still override the proxy target manually:

```powershell
$env:VITE_API_PROXY_TARGET = "http://localhost:5220"
npm run dev
```

For deployed environments, create an environment file and set:

```text
VITE_API_BASE_URL=https://claims-api.example.com
```

During the strangler migration this URL should point to the API gateway or BFF.
The gateway decides whether a capability is served by the monolith or an
extracted microservice.

The portal normalizes monolith and modular-monolith response shapes in
`src/api/claimsApi.ts`, so the same UI can be reused while backend APIs evolve.
