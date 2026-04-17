using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStore.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [MaxLength(10)]
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }

        public Guid CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
