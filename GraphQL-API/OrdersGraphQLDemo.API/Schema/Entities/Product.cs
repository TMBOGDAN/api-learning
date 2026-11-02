using System.ComponentModel.DataAnnotations;
namespace OrdersGraphQLDemo.API.Schema.Entities
{
    public class Product
    {
        [Key][MaxLength(50)] public string SkuId { get; set; } = null!;

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        [MaxLength(500)] public string Description { get; set; } = null!;

        public ICollection<OrderLine> OrderItems { get; set; } = new List<OrderLine>();
    }
}
