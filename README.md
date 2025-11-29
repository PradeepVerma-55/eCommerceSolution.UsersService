# eCommerce Users Microservice
=============================

Overview
--------
This repository implements a small microservice responsible for user management (registration, authentication) using .NET 8. It follows a clean layered structure with three main projects:

- `eeCommerce.API` — ASP.NET Core Web API (HTTP endpoints, middleware, Swagger).
- `eCommerce.Core` — Domain models, DTOs, service and repository contracts, validators, AutoMapper profiles.
- `eCommerce.Infrastructure` — Implementations (Dapper DB context, repositories), dependency injection wiring.

The service uses Dapper with `Npgsql` to talk to a PostgreSQL database, and `FluentValidation` for request validation. Swagger (Swashbuckle) is available for interactive API docs.

Goals
-----
- Provide simple REST endpoints to register and authenticate users.
- Keep business logic in the core/service layer and persistence in the infrastructure layer.
- Be easy to run locally and to reason about the code.

Prerequisites
-------------
- .NET 8 SDK
- PostgreSQL (local or remote)
- Optional: Docker (to run Postgres locally)

Repository layout
-----------------

- `eeCommerce.API/` — API project
  - `Program.cs` — app startup and middleware pipeline
  - `Middlewares/ExceptionHandlingMiddleware.cs` — centralized exception handling and logging
  - `Controllers/` — API controllers (e.g. `AuthController`)

- `eCommerce.Core/` — Core project
  - `DTO/` — request/response DTOs (e.g. `RegisterRequest`, `LoginRequest`, `AuthenticationResponse`)
  - `Entities/` — domain entities (e.g. `ApplicationUser`)
  - `RepositoryContracts/` — repository interfaces (e.g. `IUsersRepository`)
  - `ServiceContracts/` — service interfaces (e.g. `IUserService`)
  - `Services/` — service implementations (e.g. `UserService`)
  - `Validators/` — `FluentValidation` validators
  - `Mappers/` — `AutoMapper` profiles

- `eCommerce.Infrastructure/` — Infrastructure project
  - `DbContext/DapperDbContext.cs` — creates `NpgsqlConnection` instances
  - `Repositories/UsersRepository.cs` — user persistence using Dapper
  - `DependencyInjection.cs` — register infrastructure services into DI

How the pieces fit together
---------------------------
- HTTP requests hit controllers in `eeCommerce.API`.
- Controllers call services from `eCommerce.Core` via interfaces.
- Services use repository interfaces to persist/retrieve data.
- `eCommerce.Infrastructure` implements repository interfaces using Dapper and is registered in DI at startup.
- `FluentValidation` validators live in `eCommerce.Core` and are registered so model binding triggers validation automatically.
- `ExceptionHandlingMiddleware` logs exceptions and returns standardized JSON error responses.

Configuration
-------------
The application loads configuration as usual for .NET. Important settings:

- Connection string key: `ConnectionStrings:PostgreSqlConnection` — required. Example appsettings.json entry:

```json
"ConnectionStrings": {
  "PostgreSqlConnection": "Host=localhost;Port=5432;Username=postgres;Password=secret;Database=usersdb"
}
```

Database schema notes
---------------------
PostgreSQL folds unquoted identifiers to lower-case. Two options:

1. Preferred: Use lower-case (snake_case) column names and avoid quoted identifiers. Example schema:

```sql
CREATE TABLE public.users (
  user_id uuid PRIMARY KEY,
  email text NOT NULL UNIQUE,
  password text NOT NULL,
  person_name text,
  gender text
);
```

Then change queries to use `user_id`, `email`, etc.

2. If the DB was created with mixed-case quoted identifiers (e.g. `"UserId"`), keep using quotes in SQL. Example SQL used by the repository currently:

```sql
INSERT INTO public.users ("UserId", "Email", "Password", "PersonName", "Gender") VALUES (@UserId, ...);
```

Common error: `Npgsql.PostgresException: 42703: column "userid" of relation "users" does not exist` occurs when the DB column was created as quoted mixed-case (`"UserId"`) but the SQL uses an unquoted identifier (`UserId`) — PostgreSQL lower-cases it to `userid`.

Running locally
----------------
1. Ensure PostgreSQL is running and `PostgreSqlConnection` is set in `appsettings.Development.json` or environment variables.
2. Restore and build:

    dotnet restore
    dotnet build

3. Run the API project (from repository root):

    dotnet run --project eeCommerce.API

4. Open Swagger UI (if served at root) or the path configured in `Program.cs` (e.g. `https://localhost:5001/swagger/index.html`).

Swagger and API docs
--------------------
Swagger is registered in `Program.cs`. If XML documentation generation is enabled in `eeCommerce.API.csproj`, Swagger will include controller and DTO comments. By default the project includes `app.UseSwagger()` and `app.UseSwaggerUI()` so interactive documentation is available.

Validation
----------
- `FluentValidation` is used for request validation. Validators are located in `eCommerce.Core\\Validators` and `Program.cs` registers `AddFluentValidationAutoValidation()` so model validation occurs automatically during model binding for controllers.

Exception handling
------------------
- `ExceptionHandlingMiddleware` captures unhandled exceptions, logs them via `ILogger<T>`, and returns a JSON response with `Message` and `Type` fields and HTTP status code 500.

Dependency Injection
--------------------
- `eCommerce.Infrastructure.DependencyInjection` and `eCommerce.Core.DependencyInjection` (or similar) contain `IServiceCollection` extension methods to register services, repositories, AutoMapper and other dependencies. In `Program.cs` the calls `builder.Services.AddInfrastructure()` and `builder.Services.AddCore()` wire up implementations.

Persistence
-----------
- `DapperDbContext` provides a property `DbConnection` that returns a new `NpgsqlConnection` per access. Callers are responsible for opening/disposing connections (or rely on Dapper to open it implicitly when executing queries).
- `UsersRepository` uses Dapper to insert and select `ApplicationUser` records. It currently uses quoted mixed-case identifiers to match a schema created with quoted names.

Security
--------
- The repository currently stores passwords as plain text in DB columns named `Password`. This is NOT secure. You must hash and salt passwords (e.g., use `BCrypt` or `Argon2`) before persisting, and never return raw password values from APIs.

Testing
-------
- There are no unit/integration tests in the repo by default. Recommended approaches:
  - Unit test services by mocking repository interfaces (`IUsersRepository`).
  - Add integration tests that run against a test PostgreSQL instance (e.g., Docker) and exercise real queries.

Extending the service
---------------------
- Add more endpoints (e.g., user profile, password reset) in `eeCommerce.API.Controllers`.
- Move from raw Dapper to Dapper.Contrib or micro-ORMs if you want simple CRUD helpers.
- Add caching for frequently accessed data.
- Add health checks and metrics (Prometheus/OpenTelemetry).

Troubleshooting
---------------
- Error: `column "userid" of relation "users" does not exist` — see Database schema notes above.
- Connection failures — verify `PostgreSqlConnection` string, network, and that PostgreSQL allows connections from your host.
- Validation not running — ensure `FluentValidation.AspNetCore` package is installed in `eeCommerce.API` and `AddFluentValidationAutoValidation()` is called in `Program.cs`.

Docker and Containerization
---------------------------
This project already includes a Dockerfile at `eeCommerce.API/Dockerfile`. The instructions below show how to build and run an image using that Dockerfile and how to run the API together with PostgreSQL using Docker Compose.

1) Build Docker image using the existing Dockerfile

From the repository root run:

    docker build -t eecommerce-users-api:latest -f eeCommerce.API/Dockerfile .

2) Run the container (connect to an external Postgres)

If you already have PostgreSQL running on the host, run the container and pass the connection string via environment variable. On Windows/Mac use `host.docker.internal` to reach the host-side Postgres from the container:

    docker run -d --name users-api \
      -p 8080:8080 \
      -e ASPNETCORE_URLS="http://+:8080" \
      -e ConnectionStrings__PostgreSqlConnection="Host=host.docker.internal;Port=5432;Username=postgres;Password=secret;Database=usersdb" \
      eecommerce-users-api:latest

On Linux replace `host.docker.internal` with the host IP or run Postgres as a container (recommended).

3) Run API + Postgres together (recommended) using docker-compose

Create a `docker-compose.yml` at the repository root (example):

```yaml
version: '3.8'
services:
  db:
    image: postgres:15
    restart: unless-stopped
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: secret
      POSTGRES_DB: usersdb
    volumes:
      - db-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: eeCommerce.API/Dockerfile
    depends_on:
      db:
        condition: service_healthy
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: "http://+:8080"
      ConnectionStrings__PostgreSqlConnection: "Host=db;Port=5432;Username=postgres;Password=secret;Database=usersdb"
    ports:
      - "8080:8080"
    restart: on-failure

volumes:
  db-data:
```

Then start both services:

    docker compose up --build

Open the API/Swagger at: http://localhost:8080/swagger

4) Useful Docker commands

- Stop & remove container:

    docker stop users-api && docker rm users-api

- View logs:

    docker logs -f users-api
    docker compose logs -f api

- Exec shell into running container:

    docker exec -it users-api /bin/sh

- Tag & push image to registry:

    docker tag eecommerce-users-api:latest <registry>/eecommerce-users-api:1.0.0
    docker push <registry>/eecommerce-users-api:1.0.0

5) Notes & best practices

- Use environment variables or Docker secrets for DB credentials in production; avoid embedding secrets in images.
- Secure Swagger UI and restrict CORS in production environments.
- If your DB schema uses quoted mixed-case columns, ensure repository SQL matches (the repo currently uses quoted identifiers); prefer snake_case for new schemas.
- Add health checks and readiness probes for production orchestration.

Contributing
------------
- Fork the repository and create a PR with a clear description.
- Keep changes small and focused; include tests when adding behavior.

Further improvements / TODOs
---------------------------
- Hash and salt passwords before persisting.
- Add DTOs that do not expose internal fields such as `Password`.
- Add logging enrichment and structured logging (Serilog).
- Add tests and CI pipeline.
- Consider schema normalization (snake_case) to avoid quoting needs in SQL.

Contact / References
--------------------
- Official docs: .NET 8, Dapper, Npgsql, FluentValidation, Swashbuckle

License
-------
Check repository root for license file or add one before publishing.