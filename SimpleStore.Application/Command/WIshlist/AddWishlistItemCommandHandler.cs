
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Application.Command.WIshlist
{
    public record AddWishlistItemCommand(Guid UserId, Guid ProductId);
    public class AddWishlistItemCommandHandler
    {
        private readonly IWishlistRepository _repository;

        public AddWishlistItemCommandHandler(IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(AddWishlistItemCommand cmd, CancellationToken ct)
        {
            var wishlistItem = await _repository.GetByIdAsync(cmd.UserId, cmd.ProductId, ct);
            if (wishlistItem is not null)
                return DomainErrors.Wishlist.Conflict;

            var id = await _repository.AddAsync(cmd.UserId, cmd.ProductId, ct); 
            await _repository.SaveChangesAsync(ct);
            return id; 
        }
    }
}
