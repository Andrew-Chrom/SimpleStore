using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStore.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int Quantity { get; set; }
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
    }
}