using HotChocolate;
using OrdersGraphQLDemo.API.Data;
using OrdersGraphQLDemo.API.DTOs;
using OrdersGraphQLDemo.API.Schema.Entities;

namespace OrdersGraphQLDemo.API.Mutations
{
    public class Mutation
    {
        private readonly InMemoryStore _store;

        public Mutation(InMemoryStore store)
        {
            _store = store;
        }

        public Product AddProduct(ProductDto input)
        {
            ValidateProduct(input);

            lock (_store.SyncRoot)
            {
                if (_store.Products.Any(p => p.SkuId == input.SkuId))
                {
                    throw new GraphQLException("A product with this SKU already exists.");
                }

                var product = new Product
                {
                    SkuId = input.SkuId,
                    Name = input.Name,
                    Price = input.Price,
                    Description = input.Description
                };

                _store.Products.Add(product);
                return _store.CreateSnapshot().Products.Single(p => p.SkuId == product.SkuId);
            }
        }

        public Product? UpdateProduct(ProductDto input)
        {
            ValidateProduct(input);

            lock (_store.SyncRoot)
            {
                var product = _store.Products.FirstOrDefault(p => p.SkuId == input.SkuId);
                if (product == null)
                {
                    return null;
                }

                product.Name = input.Name;
                product.Price = input.Price;
                product.Description = input.Description;
                return _store.CreateSnapshot().Products.Single(p => p.SkuId == product.SkuId);
            }
        }

        public bool DeleteProduct(string skuId)
        {
            lock (_store.SyncRoot)
            {
                var product = _store.Products.FirstOrDefault(p => p.SkuId == skuId);
                if (product == null)
                {
                    return false;
                }

                if (product.OrderItems.Count > 0)
                {
                    throw new GraphQLException("Cannot delete a product referenced by order lines. Delete those lines first.");
                }

                return _store.Products.Remove(product);
            }
        }

        public Order AddOrder(OrderDto input)
        {
            ValidateOrder(input);

            lock (_store.SyncRoot)
            {
                if (_store.Orders.Any(o => o.Id == input.Id))
                {
                    throw new GraphQLException("An order with this ID already exists.");
                }

                var order = new Order { Id = input.Id };
                ApplyOrderInput(order, input);
                _store.Orders.Add(order);
                return _store.CreateSnapshot().Orders.Single(o => o.Id == order.Id);
            }
        }

        public Order? UpdateOrder(OrderDto input)
        {
            ValidateOrder(input);

            lock (_store.SyncRoot)
            {
                var order = _store.Orders.FirstOrDefault(o => o.Id == input.Id);
                if (order == null)
                {
                    return null;
                }

                ApplyOrderInput(order, input);
                return _store.CreateSnapshot().Orders.Single(o => o.Id == order.Id);
            }
        }

        public bool DeleteOrder(int id)
        {
            lock (_store.SyncRoot)
            {
                var order = _store.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                {
                    return false;
                }

                // Copy the collection because removing each line also updates OrderLines.
                foreach (var line in order.OrderLines.ToList())
                {
                    RemoveOrderLine(line);
                }

                return _store.Orders.Remove(order);
            }
        }

        public OrderLine AddOrderLine(OrderLineDto input)
        {
            ValidateOrderLine(input);

            lock (_store.SyncRoot)
            {
                if (_store.OrderLines.Any(ol => ol.LineId == input.LineId))
                {
                    throw new GraphQLException("An order line with this ID already exists.");
                }

                var order = FindOrder(input.OrderId);
                var product = FindProduct(input.SkuId);
                var line = new OrderLine
                {
                    LineId = input.LineId,
                    OrderId = input.OrderId,
                    SkuId = input.SkuId,
                    Quantity = input.Quantity,
                    order = order,
                    product = product
                };

                _store.OrderLines.Add(line);
                order.OrderLines.Add(line);
                product.OrderItems.Add(line);
                return _store.CreateSnapshot().OrderLines.Single(ol => ol.LineId == line.LineId);
            }
        }

        public OrderLine? UpdateOrderLine(OrderLineDto input)
        {
            ValidateOrderLine(input);

            lock (_store.SyncRoot)
            {
                var line = _store.OrderLines.FirstOrDefault(ol => ol.LineId == input.LineId);
                if (line == null)
                {
                    return null;
                }

                // Resolve both references before changing any existing relationship.
                var order = FindOrder(input.OrderId);
                var product = FindProduct(input.SkuId);

                line.order.OrderLines.Remove(line);
                line.product.OrderItems.Remove(line);
                line.OrderId = input.OrderId;
                line.SkuId = input.SkuId;
                line.Quantity = input.Quantity;
                line.order = order;
                line.product = product;
                order.OrderLines.Add(line);
                product.OrderItems.Add(line);
                return _store.CreateSnapshot().OrderLines.Single(ol => ol.LineId == line.LineId);
            }
        }

        public bool DeleteOrderLine(int lineId)
        {
            lock (_store.SyncRoot)
            {
                var line = _store.OrderLines.FirstOrDefault(ol => ol.LineId == lineId);
                if (line == null)
                {
                    return false;
                }

                RemoveOrderLine(line);
                return true;
            }
        }

        private Order FindOrder(int id)
        {
            return _store.Orders.FirstOrDefault(o => o.Id == id)
                ?? throw new GraphQLException("The referenced order does not exist.");
        }

        private Product FindProduct(string skuId)
        {
            return _store.Products.FirstOrDefault(p => p.SkuId == skuId)
                ?? throw new GraphQLException("The referenced product does not exist.");
        }

        private void RemoveOrderLine(OrderLine line)
        {
            line.order.OrderLines.Remove(line);
            line.product.OrderItems.Remove(line);
            _store.OrderLines.Remove(line);
        }

        private static void ApplyOrderInput(Order order, OrderDto input)
        {
            order.CustomerId = input.CustomerId;
            order.Name = input.Name;
            order.AddresssLine1 = input.AddresssLine1;
            order.AddresssLine2 = input.AddresssLine2;
            order.Country = input.Country;
            order.PostalCode = input.PostalCode;
            order.Date = input.Date;
        }

        private static void ValidateProduct(ProductDto input)
        {
            ValidateText(input.SkuId, "SkuId", 50);
            ValidateText(input.Name, "Name");
            ValidateText(input.Description, "Description", 500, allowEmpty: true);
            if (input.Price < 0)
            {
                throw new GraphQLException("Price must be zero or greater.");
            }
        }

        private static void ValidateOrder(OrderDto input)
        {
            if (input.Id <= 0)
            {
                throw new GraphQLException("Order ID must be greater than zero.");
            }

            ValidateText(input.CustomerId, "CustomerId", 200);
            ValidateText(input.Name, "Name", 200);
            ValidateText(input.AddresssLine1, "AddresssLine1", 200);
            ValidateText(input.AddresssLine2, "AddresssLine2", 200, allowEmpty: true);
            ValidateText(input.Country, "Country", 50);
            ValidateText(input.PostalCode, "PostalCode", 50);
            if (input.Date == default)
            {
                throw new GraphQLException("Date must be provided.");
            }
        }

        private static void ValidateOrderLine(OrderLineDto input)
        {
            if (input.LineId <= 0 || input.OrderId <= 0)
            {
                throw new GraphQLException("Line ID and order ID must be greater than zero.");
            }

            ValidateText(input.SkuId, "SkuId", 50);
            if (input.Quantity <= 0)
            {
                throw new GraphQLException("Quantity must be greater than zero.");
            }
        }

        private static void ValidateText(string? value, string field, int? maxLength = null, bool allowEmpty = false)
        {
            if (value == null || (!allowEmpty && string.IsNullOrWhiteSpace(value)))
            {
                throw new GraphQLException($"{field} must be provided.");
            }

            if (maxLength.HasValue && value.Length > maxLength.Value)
            {
                throw new GraphQLException($"{field} cannot exceed {maxLength.Value} characters.");
            }
        }
    }
}
