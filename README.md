# Career Path
A modular, Clean Architecture–based .NET application designed to track and manage employee career progression, including roles, reporting hierarchy, and career history within an organization.

## Project overview
CareerPath is an ASP.NET Core Web API (targeting .NET 9) using:
* Clean, layered structure: ```API, Core, Infrastructure```
* EF Core (SqlServer) for persistence
* Repository pattern
* Serilog for logging (console + file)
* Seed data for quick demo
* Unit tests (xUnit), Coverlet for coverage.
* SonarQube Integration

This repo includes a ready GitHub Actions workflow to build/publish the API and SonarQube integration (SonarCloud).

## Tech Stack
- **Language:** C#
- **Backend (API):** ASP.NET Core Web API (.NET 9), Entity Framework Core (Code-First + Migrations), Serilog for logging
- **Database:** Azure SQL Database, EF Core Migrations
- **DevOps / CI-CD:** Github actions for build, test and deploy, automated build -> publish -> deploy pipelines, secrets management via Github Environments.
- **Testing:** xUnit for unit testing, In-Memery EF Core for repository testing.
- **Architecture:** Clean architecture, Interface-driven design, dependency injection, EntityFramework Core, in-memory store (test)
- **Code Quality & Security:** SonarQube

## Project Structure
```text
CareerPath/
│
├── API/                                            # ASP.NET Core Web API project (entrypoint)
│   └── Extensions/
│   |   └── ApplicationServiceExtensions.cs
│   |   └── DateExtensions.cs
|   |   └── LoggingExtensions.cs
|   └── DTOs/
│   |   └── CareerHistoryDto.cs
│   |   └── CreateEmployeeDto.cs
│   |   └── EmployeeDto.cs
│   |   └── RoleDto.cs
│   └── Errors/
│   |   └── ApiErrorResponse.cs
│   └── logs/
│   └── Middlewares
│   |   └── ExceptionMiddleware.cs
│   └── API.http                                    # list of all api endpoints to test.
│   └── appsettings.json
│   └── appsettings.Development.json
│   └── Program.cs
│
├── Core/                                           # Domain entities, interfaces, request helpers (PagedResult, RequestParams)
│   └── Entities/
│   |   └── BaseEntity.cs
│   |   └── CareerHistory.cs
│   |   └── Employee.cs
│   |   └── Role.cs
│   └── Interfaces/
│   |   └── ICareerPathRepository.cs
│   └── RequestHelpers/
│   |   └── PagedResult.cs
│   |   └── RequestParams.cs
|
├── Infrastructure/                                 # EF Core DbContext, Repositories, Migrations, Seed data
│   └── Data/
│   |   └── SeedData/
|   |   |   └── CareerHistory.json
|   |   |   └── Employees.json
|   |   |   └── Roles.json
│   |   └── CareerPathContextSeed.cs
│   |   └── CareerPathDbContext.cs
│   └── Migrations/                                 # EF Migrations (committed)
│   └── Repositories/
│   |   └── CareerPathRepository.cs
|
├── Test/                                           # xUnit tests (Test.csproj)
│   └── TestResults/
│   └── CareerPathTests.cs                          # Repository tests
│   └── AddCoverlet_README.txt                    
│
├─ .github/workflows/                               # CI/CD workflows (build & deploy)
│   └── main.yml
│   └── sonarqube-build.yml
├─ CareerPath.sln
└─ README.md
|
```
## Local setup & run
**Prerequisites**
* .NET SDK 9.x installed
* SQL Server locally / Azure SQL connection string for production testing

**1. Clone the repo**
```
git clone https://github.com/Suraj-Varade/CareerPath-Clean-Architecture-.NET-9.git

cd CareerPath
```
- Use appsettings.Development.json for local development (do NOT commit secrets).
- Alternatively, you can aslo use dotnet user-secrets for local secrets
```text
"ConnectionStrings": {
    "DefaultConnection": "{yourDbConnectionString}"
  },
```

**2. Restore, build and run**
```text
# from repo root
dotnet restore CareerPath.sln

# build only the API project
dotnet build /API/API.csproj -c Release

# (Optional) If you want to run tests
dotnet test

# publish (local verification)
dotnet publish /API/API.csproj -c Release -o ./publish_output

# run the published app
cd publish_output

dotnet API.dll

# or from project
dotnet run --project /API/API.csproj -c Release
```

After the app starts, test:
http://localhost:5078/api/employees

**Ef migrations:**
* dotnet-ef tool if you manage migrations locally:
```text
dotnet tool install --global dotnet-ef
```

## Entity Framework, Migrations & Seeding
**Key points**
* Migrations are committed under Infrastructure/Migrations/ (so CI/CD & runtime can apply them).
* On startup the app applies migrations only via context.Database.MigrateAsync() and then runs seeding (CareerPathContextSeed.SeedAsync(context)).

**Important**: Do **not** call EnsureCreated() in production when you plan to use migrations — EnsureCreated() and Migrate() conflict and can cause errors like "There is already an object named 'Employees' in the database."

**Local migration workflow**
If you want to create/update migrations locally:
```text
# Add a migration (project containing DbContext is Infrastructure)
dotnet ef migrations add InitialCreate -p Infrastructure/Infrastructure.csproj -s API/API.csproj

# Apply locally (if connection string points to your local DB)
dotnet ef database update -p Infrastructure/Infrastructure.csproj -s API/API.csproj
```
**Running migrations during CI/CD / Deploy**
You can:
* Run dotnet ef database update as a step in your GitHub Action (recommended for dev/integration), or
* Keep context.Database.MigrateAsync() in startup (wrap in try-catch and log), but ensure migrations are committed and DB state is compatible (avoid EnsureCreated).

**Tests & Code Coverage**
* Tests are in Test/ (xUnit).
* **Important CI note**: Do not publish test artifacts to production deployment output. Publish only the API project output.
* Coverlet (code coverage) may be present; avoid instrumenting builds that produce the final publish artifact to be deployed. In CI, run tests in a separate step and don't copy bin/Debug test outputs to artifact folder.

Example test command (CI):
```
- name: Run tests
  run: dotnet test CareerPath.sln --configuration Release
```

**Code Review and Security**
* SonarQube with SonarCloud.

Ensure tests are built before running tests (remove --no-build if you want to build in the same job).

## GitHub Actions CI/CD (recommended pipeline)
High-level flow:
1. Checkout
2. Setup .NET (DOTNET_VERSION: '9.0.x')
3. Restore dependencies (solution)
4. Build API project
5. Run tests (optional; ensure tests succeed before publish)
6. Clean publish folder
7. Publish API project to publish_output


## Useful commands (summary)
Build & publish API:
```
dotnet restore CareerPath.sln
dotnet build /API/API.csproj -c Release
dotnet publish /API/API.csproj -c Release -o ./publish_output
```

Add migration (local dev):
```
dotnet ef migrations add YourMigrationName -p Infrastructure/Infrastructure.csproj -s API/API.csproj
```

Apply migrations to DB:
```
# uses connection string from appsettings or environment
dotnet ef database update -p Infrastructure/Infrastructure.csproj -s API/API.csproj
```

## Where to look in the repo
* ```API/Program.cs``` — app setup, logging, migrations, api endpoints and their configurations.
* ```Infrastructure/Migrations/``` — migration files.
* ```.github/workflows/sonarqube-build.yml``` — sonarQube definations.
* ```.github/workflows/``` — CI/CD definitions (edit to implement infra deploy step).

## Final recommended additions
* Add README.md (this file) to root.
* Add a small deploy-infra.yml workflow:
    * login with service principal,
    * run the Bicep deployment,
    * then deploy the published artifact.
