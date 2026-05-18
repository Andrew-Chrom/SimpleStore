
namespace SimpleStore.Application.Dto.Wishlist
{
    public record WishlistItemDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
    }
}
