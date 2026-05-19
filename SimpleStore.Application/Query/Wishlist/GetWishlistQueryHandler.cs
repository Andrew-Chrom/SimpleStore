

using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Wishlist;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Application.Query.Wishlist
{
    public record GetWishListQuery(Guid UserId);

    public class GetWishlistQueryHandler
    {
        private readonly IWishlistRepository _repository;

        public GetWishlistQueryHandler(IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<WishlistItemDto>>> Handle(GetWishListQuery query, CancellationToken ct)
        {
            var wishlist = await _repository.GetAllAsync(query.UserId, ct);
            return wishlist;
        }
    }
}
