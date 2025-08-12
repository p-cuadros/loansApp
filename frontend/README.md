# Frontend 

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.1.6.

## Running the Frontend

Install dependencies:
```sh
npm install
```  

Start the development server:  
```sh
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

- The app fetches loans from the backend at `http://localhost:8080/loans`.
- Run `docker compose up --build` from the repo root to start the backend and SQL Server.
