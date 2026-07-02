# Healthcare Claims Portal

The React frontend is intentionally outside every backend implementation:

```text
frontend/healthcare-claims-portal
monolith/
modular-monolith/       # added later in the course
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
npm run dev
```

Open `http://localhost:5173`. Vite proxies `/api` requests to
`http://localhost:5213`.

The monolith does not force HTTP-to-HTTPS redirects in Development, so proxied
API calls remain on the Vite origin and do not trigger browser CORS errors.

When the API is not running, the portal uses representative walkthrough data and
shows a visible **Demo mode** indicator.

## Switching backends

For deployed environments, create an environment file and set:

```text
VITE_API_BASE_URL=https://claims-api.example.com
```

During the strangler migration this URL should point to the API gateway or BFF.
The gateway decides whether a capability is served by the monolith or an
extracted microservice.
