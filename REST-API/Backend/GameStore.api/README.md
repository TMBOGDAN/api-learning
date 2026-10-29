# GameStore backend

An educational REST API built with ASP.NET Core Minimal APIs, Entity Framework Core and SQLite.

## Code map

| Location | Responsibility |
| --- | --- |
| `Program.cs` | Registers services, maps endpoints and applies database migrations |
| `Endpoints/` | Handles HTTP requests and returns responses |
| `Dtos/` | Defines request/response data contracts |
| `Models/` | Defines database entities |
| `Data/GameStoreContext.cs` | Provides access to games and genres |
| `Data/DataExtensions.cs` | Configures SQLite, migrations and initial genre data |
| `Data/Migrations/` | Records schema changes |

## Run

From the repository root with the .NET 10 SDK installed:

```powershell
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

Use `http://localhost:5187`. The database is created locally at startup.
Request examples are in [games.http](../../games.http); routes and setup are documented in the [REST API README](../../README.md).

## Key concepts

A **DTO** defines the client contract; an **entity** represents stored data.
**Dependency injection** provides a database context to handlers.
**LINQ** describes database queries; `ToListAsync()` executes a query asynchronously.
**Migrations** evolve the schema; **seeding** supplies initial data.

Read the [short theory notes](../../notes.md) for REST principles, HTTP methods, response codes and explanations of the backend concepts.
