using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStore.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public OrderStatus Status { get; set; }
        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }

    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Cancelled
    }

}
