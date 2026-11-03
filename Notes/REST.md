# REST API - GameStore

Project: [REST-API](../REST-API/). Documentation paths below are relative to the REST backend unless stated otherwise.

## Project guide

Exercitiu educational: un API REST pentru gestionarea jocurilor si citirea genurilor,
construit cu ASP.NET Core Minimal APIs, Entity Framework Core si SQLite.

### Pornire

Din radacina repository-ului, cu SDK-ul .NET 10 instalat:

```powershell
dotnet build REST-API/GameStore.slnx
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

API-ul porneste la `http://localhost:5187`.
Baza SQLite este creata local, migrarile sunt aplicate la pornire,
iar genurile initiale sunt adaugate cand tabela este goala.
Bazele de date locale si fisierele de build sunt excluse din Git.

### Endpoint-uri

| Metoda | Ruta | Scop |
| --- | --- | --- |
| GET | /games | Lista jocurilor |
| GET | /games/{id} | Un joc dupa ID |
| POST | /games | Adaugarea unui joc |
| PUT | /games/{id} | Actualizarea unui joc |
| DELETE | /games/{id} | Stergerea unui joc |
| GET | /genres | Lista genurilor si ID-urilor disponibile |

Exemplele de cereri sunt in [games.http](../REST-API/games.http). Pot fi executate cu extensia REST Client din VS Code.
Pentru creare si actualizare, foloseste un `genreID` existent din `GET /genres`.
ID-ul jocului este generat de baza de date; la actualizare este folosit ID-ul din ruta.

### Ce invat din proiect

- Rute HTTP si operatii CRUD.
- Separarea modelelor de persistenta de DTO-uri.
- Validarea datelor si coduri de raspuns HTTP.
- Injectarea dependentelor si operatii asincrone.
- Persistenta cu SQLite si migrari Entity Framework Core.

Notitele personale sunt in sectiunea [Theory notes](#theory-notes) de mai jos.

## Backend guide

An educational REST API built with ASP.NET Core Minimal APIs, Entity Framework Core and SQLite.

### Code map

| Location | Responsibility |
| --- | --- |
| `Program.cs` | Registers services, maps endpoints and applies database migrations |
| `Endpoints/` | Handles HTTP requests and returns responses |
| `Dtos/` | Defines request/response data contracts |
| `Models/` | Defines database entities |
| `Data/GameStoreContext.cs` | Provides access to games and genres |
| `Data/DataExtensions.cs` | Configures SQLite, migrations and initial genre data |
| `Data/Migrations/` | Records schema changes |

### Run

From the repository root with the .NET 10 SDK installed:

```powershell
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

Use `http://localhost:5187`. The database is created locally at startup.
Request examples are in [games.http](../REST-API/games.http); routes and setup are documented in the [project guide](#project-guide).

### Key concepts

A **DTO** defines the client contract; an **entity** represents stored data.
**Dependency injection** provides a database context to handlers.
**LINQ** describes database queries; `ToListAsync()` executes a query asynchronously.
**Migrations** evolve the schema; **seeding** supplies initial data.

Read the [short theory notes](#theory-notes) for REST principles, HTTP methods, response codes and explanations of the backend concepts.

## Theory notes

### API and REST

An API (Application Programming Interface) defines how software components communicate.
REST (Representational State Transfer) is an architectural style that models data as resources.
In this project, games and genres are resources exposed through HTTP routes.
Clients can store data too; this backend provides shared storage and applies application rules.

### REST concepts

- **Client-server:** the client handles presentation; the server handles resources and application logic.
- **Stateless:** each request contains the information needed to process it. The server may still store resource data in a database.
- **Uniform interface:** use consistent resource URLs, HTTP methods and representations such as JSON.
- **Cacheable:** responses declare whether clients or intermediaries may reuse them. Caching is not configured in this exercise.
- **Layered system:** proxies or gateways can sit between the client and server.
- **Code on demand (optional):** a server may provide executable client code; this API does not use it.

### HTTP and CRUD

A request includes a method, URL, headers and sometimes a body. A response includes a status code and optionally a body.
CRUD means Create, Read, Update and Delete.

| Method | Example | Purpose |
| --- | --- | --- |
| GET | /games/1 | Read a resource; should not modify it |
| POST | /games | Create a game; the database generates its ID |
| PUT | /games/1 | Replace the editable values of an existing game |
| DELETE | /games/1 | Remove a game |

GET is safe and idempotent. PUT and DELETE are idempotent: repeating the same request has the same intended effect on server state, even if response codes differ. POST is generally not idempotent.

Common responses: `200 OK` for reads, `201 Created` with a Location header after creation,
`204 No Content` after update/delete, `400 Bad Request` for invalid input and `404 Not Found` for a missing resource.

### Backend concepts in this project

- **Endpoint:** a route plus a handler, registered with methods such as `MapGet` or `MapPost`.
- **DTO:** a Data Transfer Object describes the data exchanged with clients. `GamesDto` and `GenreDto` are API contracts.
- **Entity:** a persistence model, such as `Games` or `Genre`, mapped to database data. Entities and DTOs have different responsibilities.
- **Validation:** data annotations check request values. Handlers also check that the supplied genre exists.
- **Dependency injection:** ASP.NET Core supplies `GameStoreContext` to each endpoint instead of the handler constructing it.
- **ORM:** Entity Framework Core maps C# objects and LINQ queries to relational database operations.
- **DbContext / DbSet:** the context manages database access and tracked changes; a DbSet is an entry point for querying and modifying an entity type.
- **LINQ / Select:** LINQ builds queries; `Select` projects entities into DTOs. A query is executed when materialized, for example with `ToListAsync()`.
- **AsNoTracking:** skips change tracking for read-only queries, reducing tracking overhead.
- **Async / await:** asynchronous database calls let the request wait for I/O without blocking a thread. `await` needs an awaitable operation, not an `IQueryable`.
- **SaveChangesAsync:** persists tracked additions, updates and deletions to the database.
- **SQLite:** a relational database stored in a local file. Each developer creates their own database; database files are excluded from Git.
- **Migration:** a versioned database schema change. This application applies migrations at startup.
- **Seeding:** adds initial data, such as genres, when the relevant table is empty.

### Request flow

```text
HTTP request -> route handler -> validation -> DbContext -> SQLite
HTTP response <- status code and DTO <- handler <- database result
```

### Current design choices

The handlers use the database for all game CRUD operations. `GET /genres` supplies valid genre IDs.
The game DTO currently contains both `Genre` and `GenreID`; handlers use `GenreID` to look up the stored genre name.
A future exercise could introduce separate create/update DTOs without response-only fields such as `Id`.
