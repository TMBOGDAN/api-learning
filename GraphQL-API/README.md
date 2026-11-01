# Orders GraphQL Demo

Work-in-progress ASP.NET Core project for learning GraphQL with Hot Chocolate.

Requires the **.NET 11 SDK** (preview).

Run from the repository root:

```powershell
dotnet build GraphQL-API/OrdersGraphQLDemo.slnx
dotnet run --project GraphQL-API/OrdersGraphQLDemo.API --launch-profile http
```

The current application exposes `GET /`, returning `Hello World!`. GraphQL services and an endpoint are not configured yet.
