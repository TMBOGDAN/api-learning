# API Learning

A personal repository for learning different API styles using **C# and .NET**.

Each project includes practical examples. Project guides, READMEs, and study notes are collected in [Notes](Notes/).

## Projects

| Project | Description | Guide and notes |
| --- | --- | --- |
| [REST API](REST-API/) | GameStore - CRUD operations, HTTP, DTOs, validation, Entity Framework Core, and SQLite | [REST](Notes/REST.md) |
| [GraphQL API](GraphQL-API/) | OrdersGraphQLDemo - queries, DTO-based CRUD mutations, and shared in-memory data with Hot Chocolate | [GraphQL](Notes/GraphQL.md) |
| Next project | Reserved for the next API project | [Next project template](Notes/Next-Project.md) |

More projects, including **gRPC**, will be added in the future.

## Getting Started

The REST project requires **.NET 10 SDK**. The GraphQL project requires **.NET 11 SDK** (preview).

```powershell
dotnet build REST-API/GameStore.slnx
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

For GraphQL:

```powershell
dotnet build GraphQL-API/OrdersGraphQLDemo.slnx
dotnet run --project GraphQL-API/OrdersGraphQLDemo.API --launch-profile http
```

See [REST notes](Notes/REST.md) and [GraphQL notes](Notes/GraphQL.md) for endpoints, examples, concepts, and test instructions. Use [Next-Project.md](Notes/Next-Project.md) when adding another project.

> This repository is for educational purposes and is a work in progress.
