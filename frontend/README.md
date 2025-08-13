# Frontend 

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.1.6.

## Running the Frontend

Install dependencies:
```pwsh
npm install
```  

Start the development server:  
```pwsh
npm start
```

Open `http://localhost:4200/` in your browser.

End-to-end tests (Playwright)

1. Install dependencies and Playwright browsers:
```sh
npm install
npm run e2e:install
```
2. Run the tests:
```sh
npm run e2e
```
3. Optional UI mode:
```sh
npm run e2e:ui
```

Notes

- When running on port 4200, the app points to `http://localhost:8080` for the API.
- Use the Login screen (admin/admin) before creating loans or making payments.
- A lightweight metrics post is sent to the backend at `/metrics/http-client` after each request for observability.
- Run `docker compose up --build` from the repo root to start the backend, SQL Server, Loki, and Grafana.
