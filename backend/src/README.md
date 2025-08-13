## Running the Backend

To build the backend, navigate to the `src` folder and run:  
```sh
dotnet build
```

To run all tests:  
```sh
dotnet test
```

To start the main API locally without Docker:
```pwsh
cd Fundo.Applications.WebApi
dotnet run
```

Default ports when running via Docker Compose:
- API: http://localhost:8080
- SQL Server: localhost,1433 (sa/Your_password123)

### Docker (recommended)

Run the API and SQL Server locally with Docker:

```pwsh
docker compose up --build
```

Once the containers are healthy:
- API: http://localhost:8080/loans

### Authentication

- Login: POST http://localhost:8080/auth/login (admin/admin)
- Include Authorization: Bearer <token> for POST endpoints on /loans

### Observability

- Logs are sent to console and Loki (if LOKI_URL env set).
- Grafana: http://localhost:3000, with a simplified metrics dashboard (3 panels) and a logs dashboard.

## Notes  

Feel free to modify the code as needed, but try to **respect and extend the current architecture**, as this is intended to be a replica of the Fundo codebase.
