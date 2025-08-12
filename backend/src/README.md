## Running the Backend

To build the backend, navigate to the `src` folder and run:  
```sh
dotnet build
```

To run all tests:  
```sh
dotnet test
```

To start the main API:  
```sh
cd Fundo.Applications.WebApi  
dotnet run
```

The following endpoint should return **200 OK**:  
```http
GET -> https://localhost:5001/loans
```

### Docker (recommended)

Run the API and SQL Server locally with Docker:

```sh
docker compose up --build
```

Once the containers are healthy:
- API: http://localhost:8080/loans

## Notes  

Feel free to modify the code as needed, but try to **respect and extend the current architecture**, as this is intended to be a replica of the Fundo codebase.
