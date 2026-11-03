namespace OrdersGraphQLDemo.API.DTOs
{
    public class ProductDto
    {
        public string SkuId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
    }
}
