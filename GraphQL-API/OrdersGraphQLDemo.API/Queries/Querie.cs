using OrdersGraphQLDemo.API.Schema.Entities;
namespace OrdersGraphQLDemo.API.Queries
{
    public class Query
    {
        public string Hello() => "Hello World!";

        private readonly List<Product> _products;
        private readonly List<Order> _Orders;
        private readonly List<OrderLine> _OrderLines;
        public Query()
        {
            _products = new List<Product>
            {
                new Product { SkuId = "BOOK-001", Name = "C# Basics", Price = 29.99m, Description = "An introduction to C#" },
                new Product { SkuId = "BOOK-002", Name = "Learning GraphQL", Price = 34.99m, Description = "Building GraphQL APIs" },
                new Product { SkuId = "BOOK-003", Name = "ASP.NET Core", Price = 39.99m, Description = "Building web applications" },
                new Product { SkuId = "BOOK-004", Name = "Database Design", Price = 44.99m, Description = "Relational database fundamentals" },
                new Product { SkuId = "BOOK-005", Name = "Clean Code", Price = 32.99m, Description = "Writing maintainable software" },
                new Product { SkuId = "BOOK-006", Name = "Unit Testing", Price = 27.99m, Description = "Testing applications" },
                new Product { SkuId = "BOOK-007", Name = "Design Patterns", Price = 49.99m, Description = "Common software design patterns" },
                new Product { SkuId = "BOOK-008", Name = "Distributed Systems", Price = 54.99m, Description = "Building distributed applications" },
                new Product { SkuId = "BOOK-009", Name = "API Design", Price = 36.99m, Description = "Designing web APIs" },
                new Product { SkuId = "BOOK-010", Name = "Software Architecture", Price = 59.99m, Description = "Structuring software systems" }
            };

            _Orders = new List<Order>();
            var customerNames = new[]
            {
                "Andrei Popescu",
                "Maria Ionescu",
                "Alexandru Dumitrescu",
                "Elena Stan",
                "Mihai Georgescu"
            };

            for (var i = 1; i <= 5; i++)
            {
                var order = new Order
                {
                    Id = i,
                    CustomerId = $"CUSTOMER-{i:000}",
                    Name = customerNames[i - 1],
                    AddresssLine1 = $"{i * 10} Main Street",
                    AddresssLine2 = $"Apartment {i}",
                    Country = "Romania",
                    PostalCode = $"01000{i}",
                    Date = new DateTime(2026, 11, i, 10, 0, 0, DateTimeKind.Utc)
                };
                _Orders.Add(order);
            }

            _OrderLines = new List<OrderLine>
            {
                // Order 1 (2 lines)
                new OrderLine { LineId = 1, OrderId = 1, SkuId = "BOOK-001", Quantity = 1 },
                new OrderLine { LineId = 2, OrderId = 1, SkuId = "BOOK-002", Quantity = 2 },

                // Order 2 (3 lines)
                new OrderLine { LineId = 3, OrderId = 2, SkuId = "BOOK-003", Quantity = 1 },
                new OrderLine { LineId = 4, OrderId = 2, SkuId = "BOOK-004", Quantity = 1 },
                new OrderLine { LineId = 5, OrderId = 2, SkuId = "BOOK-005", Quantity = 2 },

                // Order 3 (3 lines)
                new OrderLine { LineId = 6, OrderId = 3, SkuId = "BOOK-006", Quantity = 1 },
                new OrderLine { LineId = 7, OrderId = 3, SkuId = "BOOK-007", Quantity = 2 },
                new OrderLine { LineId = 8, OrderId = 3, SkuId = "BOOK-008", Quantity = 1 },

                // Order 4 (5 lines)
                new OrderLine { LineId = 9, OrderId = 4, SkuId = "BOOK-001", Quantity = 1 },
                new OrderLine { LineId = 10, OrderId = 4, SkuId = "BOOK-002", Quantity = 2 },
                new OrderLine { LineId = 11, OrderId = 4, SkuId = "BOOK-003", Quantity = 1 },
                new OrderLine { LineId = 12, OrderId = 4, SkuId = "BOOK-005", Quantity = 1 },
                new OrderLine { LineId = 13, OrderId = 4, SkuId = "BOOK-010", Quantity = 1 },

                // Order 5 (8 lines)
                new OrderLine { LineId = 14, OrderId = 5, SkuId = "BOOK-001", Quantity = 1 },
                new OrderLine { LineId = 15, OrderId = 5, SkuId = "BOOK-002", Quantity = 1 },
                new OrderLine { LineId = 16, OrderId = 5, SkuId = "BOOK-003", Quantity = 1 },
                new OrderLine { LineId = 17, OrderId = 5, SkuId = "BOOK-004", Quantity = 1 },
                new OrderLine { LineId = 18, OrderId = 5, SkuId = "BOOK-005", Quantity = 1 },
                new OrderLine { LineId = 19, OrderId = 5, SkuId = "BOOK-006", Quantity = 1 },
                new OrderLine { LineId = 20, OrderId = 5, SkuId = "BOOK-007", Quantity = 3 },
                new OrderLine { LineId = 21, OrderId = 5, SkuId = "BOOK-008", Quantity = 2 }
            };

            foreach (var line in _OrderLines)
            {
                line.order = _Orders.Single(o => o.Id == line.OrderId);
                line.product = _products.Single(p => p.SkuId == line.SkuId);

                line.order.OrderLines.Add(line);
                line.product.OrderItems.Add(line);
            }

        }

        public IEnumerable<Product> GetProducts()
        {
            return _products;
        }

        public IEnumerable<Order> GetOrders()
        {
            return _Orders;
        }

    }
}
