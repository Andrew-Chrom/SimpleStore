

using SimpleStore.Domain.Enum;

namespace SimpleStore.Application.Dto.Orders
{
    public record PaymentEvent 
    {
        public PaymentEventEnum Event { get; set; }
        public Guid OrderId { get; set; }
    }
}
