# Orders GraphQL Demo — Quick Sheet

## About the project

Project: [GraphQL-API](../GraphQL-API/). File paths below are relative to `GraphQL-API/OrdersGraphQLDemo.API`.

A learning project for GraphQL built with ASP.NET Core, C#, and Hot Chocolate.

- **Required SDK:** .NET 11 Preview
- **Hot Chocolate:** version 15.1.14
- **Data:** Stored in memory; no database is configured

## Run the project

From the repository root, run:

```powershell
dotnet build GraphQL-API/OrdersGraphQLDemo.slnx
dotnet run --project GraphQL-API/OrdersGraphQLDemo.API --launch-profile http
```

Then open:

```text
http://localhost:5254/graphql
```

The project creates sample data: 10 products, 5 orders, and 21 order lines.

## Important files

| File                    | Purpose                                                  |
| ----------------------- | -------------------------------------------------------- |
| `Program.cs`            | Configures and starts the application                    |
| `Data/InMemoryStore.cs` | Seeds and shares data, and creates response snapshots     |
| `Queries/Querie.cs`     | Reads products, orders, and order lines                  |
| `Mutations/Mutation.cs` | Adds, updates, and deletes all three entity types         |
| `Schema/Entities/`      | Contains the `Product`, `Order`, and `OrderLine` classes |
| `DTOs/`                 | Contains objects used to send data to mutations          |
| `.csproj` file          | Configures the project and NuGet packages                |

## C# concepts used

- **Classes and properties:** Describe products, orders, and order lines.
- **Constructor:** Initializes the sample data.
- **Collections:** `List<T>` stores items; `ICollection<T>` represents a collection that can be modified.
- **Methods:** Perform operations and can return results.
- **Lambda:** A short function, such as `p => p.SkuId == skuId`.
- **LINQ:** Searches collections:
  - `FirstOrDefault` returns the first match or `null`.
  - `Single` requires exactly one match.
  - `Where` returns all matching items.
- **Nullable types:** The `?` in `Product?` means the value may be `null`.
- **String interpolation:** `$"CUSTOMER-{i:000}"` creates text using values.
- **Dependency injection:** `Query(InMemoryStore store)` receives the shared store registered with `builder.Services.AddSingleton<InMemoryStore>()` in `Program.cs`.
- **Lock:** `lock (_store.SyncRoot) { ... }` protects shared data while mutations change it.
- **Snapshot:** `CreateSnapshot()` copies the object graph for a response, so later mutations cannot change objects being serialized.

## GraphQL

The application exposes the `/graphql` endpoint. Methods in the `Query` class become GraphQL fields.

| Entity | Available query fields |
| --- | --- |
| Product | `products`, `productBySkuId(skuId: "BOOK-001")` |
| Order | `orders`, `orderById(id: 1)`, `ordersByCustomerId(customerId: "CUSTOMER-001")` |
| Order line | `orderLines`, `orderLineById(lineId: 1)`, `orderLinesByOrderId(orderId: 1)`, `orderLinesBySkuId(skuId: "BOOK-001")` |

A missing single item returns `null`; a list query with no matches returns an empty list. The `hello` field also remains available.

Example query:

```graphql
query {
  products {
    skuId
    name
    price
  }
}
```

You can also request related data, such as an order, its lines, and the associated products:

```graphql
query {
  orderById(id: 1) {
    orderLines {
      quantity
      product {
        name
      }
    }
  }
}
```

## Mutations and DTOs

A **DTO** groups the data sent to an operation. For example, `ProductDto` contains the information needed to add or update a product.

Queries and mutations use the same singleton `InMemoryStore`. Changes made through a mutation are visible to subsequent queries until the application stops.

| Entity | Add | Update | Delete |
| --- | --- | --- | --- |
| Product | `addProduct(input: ProductDtoInput!)` | `updateProduct(input: ProductDtoInput!)` | `deleteProduct(skuId: String!)` |
| Order | `addOrder(input: OrderDtoInput!)` | `updateOrder(input: OrderDtoInput!)` | `deleteOrder(id: Int!)` |
| Order line | `addOrderLine(input: OrderLineDtoInput!)` | `updateOrderLine(input: OrderLineDtoInput!)` | `deleteOrderLine(lineId: Int!)` |

The table uses GraphQL schema notation: `!` means a required value. In an operation, supply an input object or a variable as shown below.

```graphql
mutation {
  addProduct(input: {
    skuId: "BOOK-011"
    name: "Learning C#"
    price: 24.99
    description: "A practical introduction"
  }) {
    skuId
    name
    price
  }
}
```

```graphql
mutation {
  addOrder(input: {
    id: 6
    customerId: "CUSTOMER-006"
    name: "New customer"
    addresssLine1: "60 Main Street"
    addresssLine2: ""
    country: "Romania"
    postalCode: "010006"
    date: "2026-11-03T10:00:00Z"
  }) {
    id
    name
  }
}
```

After creating that product and order:

```graphql
mutation {
  addOrderLine(input: {
    lineId: 22
    orderId: 6
    skuId: "BOOK-011"
    quantity: 2
  }) {
    lineId
    quantity
    order { id }
    product { name }
  }
}
```

- Add and update methods accept the corresponding DTO. Updates replace every editable scalar field; provide the complete input. IDs identify existing items and are not renamed by updates.
- The caller supplies unique product SKUs, order IDs, and line IDs. Numeric IDs and quantities must be greater than zero; prices may be zero but cannot be negative.
- Required text fields cannot be blank, and declared maximum lengths are checked. A description and second address line may be empty strings. Order dates must be provided.
- An order line must reference an existing order and product. Updating it can move it to another order or product, updating both navigation collections.
- Deleting an order also deletes its lines. Deleting a line removes it from both its order and product. A product referenced by any line cannot be deleted until those lines are removed.
- An update to a missing item returns `null`. Deletes return `true` when an item was removed and `false` when it did not exist.
- Duplicate IDs and invalid input produce GraphQL errors without modifying the stored data.

## CRUD integration tests

After building, run the tests with Python 3 (standard library only):

```powershell
python -B -m unittest discover -s GraphQL-API/tests -v
```

[The tests](../GraphQL-API/tests/test_graphql_crud.py) start and stop an isolated API process on an available local port. They verify CRUD operations across requests, nested relationships, moving lines, deletion cleanup, missing records, and rejected invalid inputs.

## Limitation

Data is lost when the application stops because the project does not use a database.
