using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OrdersGraphQLDemo.API.Schema.Entities
{
    public class OrderLine
    {
        [Key] public int LineId { get; set; }
        [ForeignKey(nameof(Order))] public int OrderId { get; set; }
        [ForeignKey(nameof(Product))] public string SkuId { get; set; } = null!;

        public int Quantity { get; set; }
        public Order order { get; set; } = null!;
        public Product product { get; set; } = null!;

    }
}
