using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Dto.Cart
{
    public record CartResponse()
    {
        public List<CartItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
