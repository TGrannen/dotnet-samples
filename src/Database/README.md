# Database

A minimal .NET 9 FluentMigrator runner for PostgreSQL. It scans the assembly for migrations and applies any pending “Up” migrations at startup. Configuration and logging are
handled via appsettings and Serilog.

## Solution overview

- Database.sln — Solution file
- docker-compose.yml — Local PostgreSQL via Docker
- FluentMigratorSample.Runner/
    - Migrations/ — C# migration classes (Up/Down)
    - Program.cs — Host bootstrap that runs migrations on startup
    - appsettings.json — Connection string and Serilog configuration

Key technologies:

- .NET SDK 9
- FluentMigrator (assembly scanning + IMigrationRunner)
- PostgreSQL
- Serilog (console sink)

## Prerequisites

- .NET SDK 9.x
- Docker Desktop (or Docker Engine)
- Optional: psql client for verification

## Quick start

1) Start PostgreSQL with Docker
    - Command: docker compose up -d
    - The container exposes port 5432. Default credentials/database are defined in docker-compose.yml.

2) Configure the connection string
    - Default path: FluentMigratorSample.Runner/appsettings.json
    - Key: ConnectionStrings:DefaultConnection
    - You can override via environment variable:
        - Name: ConnectionStrings__DefaultConnection

3) Restore and build
    - dotnet restore
    - dotnet build

4) Run migrations
    - dotnet run --project FluentMigratorSample.Runner
    - Behavior:
        - If there are pending migrations, the runner logs “Starting Migration”, applies them, and logs “Migration Complete”.
        - If not, it logs “No migrations to apply”.

5) Verify (optional)
    - Connect using psql or a GUI to confirm tables/changes were created.

## Adding new migrations

- Create a new C# class in FluentMigratorSample.Runner/Migrations.
- Use the FluentMigrator Migration attribute with a monotonically increasing version (often a timestamp).
- Implement Up for forward changes and Down for rollback.

Example structure:

- File name convention: `yyyymmddHHMM_DescriptiveName.cs`
- Class:

```csharp
    [Migration(yyyymmddHHMM)] 
    public sealed class DescriptiveName : Migration {
    
        public override void Up() { /* create/alter */ }
        public override void Down() { /* drop/revert */ }
    }
  ```

Notes:

- The runner scans the assembly to discover migrations and applies them in version order.
- This runner applies “Up” migrations only. To support rollbacks, extend the runner to invoke MigrateDown or use a dedicated migration tool/step.

## Configuration

- Connection string:
    - appsettings.json: ConnectionStrings:DefaultConnection
    - Override via environment variables for local/CI/CD:
        - ConnectionStrings__DefaultConnection
- Logging (Serilog):
    - Console sink enabled
    - MinimumLevel configured with sensible defaults; adjust in appsettings.json as needed.

## Local development workflow

- Start the database: docker compose up -d
- Make migration changes in Migrations/
- Run the runner:
    - dotnet run --project FluentMigratorSample.Runner
- Iterate on schema and application code as needed.

## CI/CD (suggested)

- Build and test
- Provision target database (or ensure connectivity)
- Execute the runner before deploying app code that depends on the new schema
- Ensure observability via logs; fail the pipeline on migration errors

## Troubleshooting

- Connection refused / timeout:
    - Ensure Docker container is running: docker ps
    - Confirm port 5432 is not blocked
    - Verify the connection string host/port match your environment

- Authentication failed:
    - Ensure credentials in docker-compose.yml match those in your connection string
    - If overriding with env vars, confirm the exact variable name and value

- “No migrations to apply”:
    - Confirm your new class has the Migration attribute and a unique, higher version
    - Ensure the class is in the assembly scanned by the runner (the Migrations folder within the project)

- SQL errors during Up:
    - Fix the migration and re-run. If partial changes were applied, you may need a manual fix or to implement a Down migration for rollback in development.
