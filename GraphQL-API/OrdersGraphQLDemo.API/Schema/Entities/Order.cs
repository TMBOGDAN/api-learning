using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OrdersGraphQLDemo.API.Schema.Entities
{
    public class Order
    {
        [Key] public int Id { get; set; }
        [MaxLength(200)] public string CustomerId { get; set; } = null!;
        [MaxLength(200)] public string Name { get; set; } = null!;

        [MaxLength(200)] public string AddresssLine1 { get; set; } = null!;
        [MaxLength(200)] public string AddresssLine2 { get; set; } = null!;

        [MaxLength(50)] public string Country { get; set; } = null!;
        [MaxLength(50)] public string PostalCode { get; set; } = null!;

        public DateTime Date { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    }
}