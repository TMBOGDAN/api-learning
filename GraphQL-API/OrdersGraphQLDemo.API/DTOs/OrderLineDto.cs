namespace OrdersGraphQLDemo.API.DTOs
{
    public class OrderLineDto
    {
        public int LineId { get; set; }
        public int OrderId { get; set; }
        public string SkuId { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
