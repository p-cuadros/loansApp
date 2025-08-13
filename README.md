# **Take-Home Test: Backend-Focused Full-Stack Developer (.NET C# & Angular)**

## **Objective**

This take-home test evaluates your ability to develop and integrate a .NET Core (C#) backend with an Angular frontend, focusing on API design, database integration, and basic DevOps practices.

## **Instructions**

1.  **Fork the provided repository** before starting the implementation.
2.  Implement the requested features in your forked repository.
3.  Once you have completed the implementation, **send the link** to your forked repository via email for review.

## **Task**

You will build a simple **Loan Management System** with a **.NET Core backend (C#)** exposing RESTful APIs and a **basic Angular frontend** consuming these APIs.

---

## **Requirements**

### **1. Backend (API) - .NET Core**

* Create a **RESTful API** in .NET Core to handle **loan applications**.
* Implement the following endpoints:
    * `POST /loans` → Create a new loan.
    * `GET /loans/{id}` → Retrieve loan details.
    * `GET /loans` → List all loans.
    * `POST /loans/{id}/payment` → Deduct from `currentBalance`.
* Loan example (feel free to improve it):

    ```json
    {
        "amount": 1500.00, // Amount requested
        "currentBalance": 500.00, // Remaining balance
        "applicantName": "Maria Silva", // User name
        "status": "active" // Status can be active or paid
    }
    ```

* Use **Entity Framework Core** with **SQL Server**.
* Create seed data to populate the loans (the frontend will consume this).
* Write **unit/integration tests for the API** (xUnit or NUnit).
* **Dockerize** the backend and create a **Docker Compose** file.
* Create a README with setup instructions.

### **2. Frontend - Angular (Simplified UI)**  

Develop a **lightweight Angular app** to interact with the backend

#### **Features:**  
- A **table** to display a list of existing loans.  

#### **Mockup:**  
[View Mockup](https://kzmgtjqt0vx63yji8h9l.lite.vusercontent.net/)  
(*The design doesn’t need to be an exact replica of the mockup—it serves as a reference. Aim to keep it as close as possible.*)  

---

## **Bonus (Optional, Not Required)**

* **Improve error handling and logging** with structured logs.
    - Backend: Serilog JSON logs, correlation ID middleware (X-Correlation-Id), request logging, and global exception handler returning structured errors with correlationId.
    - Frontend: HTTP interceptor adds X-Correlation-Id per request and logs structured errors to console; user sees a friendly message.
* Implement **authentication**.
* Create a **GitHub Actions** pipeline for building and testing the backend.

---

## **Evaluation Criteria**

✔ **Code quality** (clean architecture, modularization, best practices).

✔ **Functionality** (the API and frontend should work as expected).

✔ **Security considerations** (authentication, validation, secure API handling).

✔ **Testing coverage** (unit tests for critical backend functions).

✔ **Basic DevOps implementation** (Docker for backend).

---

## **Additional Information**

Candidates are encouraged to include a `README.md` file in their repository detailing their implementation approach, any challenges they faced, features they couldn't complete, and any improvements they would make given more time. Ideally, the implementation should be completed within **two days** of starting the test.

---

How to run locally

- Run with Docker Compose (recommended):
        - Prerequisites: Docker Desktop running; free ports 8080 (API), 4200 (Frontend), 3000 (Grafana).
        - From the repository root:
            - docker compose up --build -d
        - URLs once up:
            - API: http://localhost:8080
            - Frontend: http://localhost:4200
            - Grafana: http://localhost:3000 (login: admin / admin)
        - Login credentials (for protected POST endpoints): admin / admin
        - Quick API test:
            - PowerShell: Invoke-RestMethod http://localhost:8080/loans -Method GET
        - Stop the stack:
            - docker compose down
        - Troubleshooting:
            - If Grafana shows no data, generate traffic (browse the app, call GET /loans) and optionally restart Grafana: docker compose restart grafana
            - After backend code changes, rebuild just the API image: docker compose build backend && docker compose up -d

- Frontend (manual dev mode):
        - Open terminal in frontend folder
        - npm install
        - npm start
        - Open http://localhost:4200 (targets http://localhost:8080 when running on 4200)