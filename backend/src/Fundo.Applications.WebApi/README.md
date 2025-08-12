Backend API

Run locally without Docker

```sh
dotnet build
cd backend/src/Fundo.Applications.WebApi
dotnet run
```

Docker Compose (recommended)

From repo root:

```sh
docker compose up --build
```

API endpoints

- GET /loans
- GET /loans/{id}
- POST /loans
- POST /loans/{id}/payment

Logging

- Serilog is configured to output JSON logs to console.
- To also ship logs to Grafana Loki, set the environment variable `LOKI_URL` with your Loki push endpoint before starting the API.
	- Example: `LOKI_URL=http://localhost:3100` (for local Loki)
