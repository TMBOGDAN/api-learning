using OrdersGraphQLDemo.API.Data;
using OrdersGraphQLDemo.API.Schema.Entities;

namespace OrdersGraphQLDemo.API.Queries
{
    public class Query
    {
        private readonly InMemoryStore _store;

        public Query(InMemoryStore store)
        {
            _store = store;
        }

        public string Hello() => "Hello World!";

        public IEnumerable<Product> GetProducts()
        {
            return _store.CreateSnapshot().Products;
        }

        public Product? GetProductBySkuId(string skuId)
        {
            return _store.CreateSnapshot().Products.FirstOrDefault(p => p.SkuId == skuId);
        }

        public IEnumerable<Order> GetOrders()
        {
            return _store.CreateSnapshot().Orders;
        }

        public Order? GetOrderById(int id)
        {
            return _store.CreateSnapshot().Orders.FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> GetOrdersByCustomerId(string customerId)
        {
            return _store.CreateSnapshot().Orders.Where(o => o.CustomerId == customerId);
        }

        public IEnumerable<OrderLine> GetOrderLines()
        {
            return _store.CreateSnapshot().OrderLines;
        }

        public OrderLine? GetOrderLineById(int lineId)
        {
            return _store.CreateSnapshot().OrderLines.FirstOrDefault(ol => ol.LineId == lineId);
        }

        public IEnumerable<OrderLine> GetOrderLinesByOrderId(int orderId)
        {
            return _store.CreateSnapshot().OrderLines.Where(ol => ol.OrderId == orderId);
        }

        public IEnumerable<OrderLine> GetOrderLinesBySkuId(string skuId)
        {
            return _store.CreateSnapshot().OrderLines.Where(ol => ol.SkuId == skuId);
        }
    }
}
