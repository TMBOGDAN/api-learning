# API Learning

A personal repository for learning different API styles using **C# and .NET**.

Each project includes practical examples and study notes.

## Projects

| Project | Description |
|---------|-------------|
| [REST API](REST-API/) | GameStore – CRUD operations, HTTP, DTOs, validation, Entity Framework Core, and SQLite |
| [GraphQL API](GraphQL-API/) | OrdersGraphQLDemo - ASP.NET Core starter with Hot Chocolate dependencies; work in progress |

More projects, including **gRPC**, will be added in the future.

## Getting Started

The REST project requires **.NET 10 SDK**. The GraphQL project requires **.NET 11 SDK** (preview).

```powershell
dotnet build REST-API/GameStore.slnx
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

> This repository is for educational purposes and is a work in progress.
