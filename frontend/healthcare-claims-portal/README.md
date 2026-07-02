# ClaimSphere React Portal

ClaimSphere is the shared healthcare claims operations UI used throughout the
modernization course. It is a React, TypeScript, and Vite application.

## Start The Portal

From the repository root:

```powershell
cd .\frontend\healthcare-claims-portal
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

For live data, start the monolith in a separate terminal:

```powershell
dotnet run --project .\monolith\src\HealthCare.Claims.Monolith\HealthCare.Claims.Monolith.csproj
```

The development server proxies `/api/*` requests to
`http://localhost:5213`.

The .NET API keeps HTTP enabled without redirecting to HTTPS in Development.
This prevents the browser from following proxied requests to
`https://localhost:7200`, which would otherwise become cross-origin.

## Portal Modes

- **Live mode:** Uses the running .NET backend.
- **Demo mode:** Uses representative local data when the backend is unavailable.

Use the refresh button in the header after starting or stopping the backend.

## Backend Boundary

All backend communication is isolated in:

```text
src/api/claimsApi.ts
```

The UI consumes stable TypeScript contracts from:

```text
src/api/types.ts
```

This boundary allows the course to replace the monolith with a modular monolith,
API gateway, and extracted microservices without rewriting the React views.

To use a deployed backend, create `.env.local`:

```text
VITE_API_BASE_URL=https://claims-api.example.com
```

Restart Vite after changing environment variables.

## Commands

```powershell
npm run dev
npm run build
npm run lint
npm run preview
```

Stop a running development server with `Ctrl+C`.
