
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Application.Command.WIshlist
{
    public record RemoveWishlistItemCommand(Guid UserId, Guid ProductId);
    public class RemoveWishlistItemCommandHandler
    {
        private readonly IWishlistRepository _repository;

        public RemoveWishlistItemCommandHandler(IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(RemoveWishlistItemCommand cmd, CancellationToken ct)
        {
            if (await _repository.GetByIdAsync(cmd.UserId, cmd.ProductId, ct) is null)
                return DomainErrors.Wishlist.Conflict;
                
            await _repository.RemoveAsync(cmd.UserId, cmd.ProductId, ct);
            await _repository.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
