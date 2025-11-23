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