# REST API: short theory notes

## API and REST

An API (Application Programming Interface) defines how software components communicate.
REST (Representational State Transfer) is an architectural style that models data as resources.
In this project, games and genres are resources exposed through HTTP routes.
Clients can store data too; this backend provides shared storage and applies application rules.

## REST concepts

- **Client-server:** the client handles presentation; the server handles resources and application logic.
- **Stateless:** each request contains the information needed to process it. The server may still store resource data in a database.
- **Uniform interface:** use consistent resource URLs, HTTP methods and representations such as JSON.
- **Cacheable:** responses declare whether clients or intermediaries may reuse them. Caching is not configured in this exercise.
- **Layered system:** proxies or gateways can sit between the client and server.
- **Code on demand (optional):** a server may provide executable client code; this API does not use it.

## HTTP and CRUD

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

## Backend concepts in this project

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

## Request flow

```text
HTTP request -> route handler -> validation -> DbContext -> SQLite
HTTP response <- status code and DTO <- handler <- database result
```

## Current design choices

The handlers use the database for all game CRUD operations. `GET /genres` supplies valid genre IDs.
The game DTO currently contains both `Genre` and `GenreID`; handlers use `GenreID` to look up the stored genre name.
A future exercise could introduce separate create/update DTOs without response-only fields such as `Id`.
