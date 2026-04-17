using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStore.Domain.Entities
{
    public class WishlistItem
    {
        [Key]
        public Guid Id {  get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }   
}