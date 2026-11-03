namespace OrdersGraphQLDemo.API.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string AddresssLine1 { get; set; } = null!;
        public string AddresssLine2 { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
