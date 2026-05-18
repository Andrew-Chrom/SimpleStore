
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

        public async Task Handle(RemoveWishlistItemCommand cmd, CancellationToken ct)
        {
            await _repository.RemoveAsync(cmd.UserId, cmd.ProductId, ct);
            await _repository.SaveChangesAsync(ct);
        }
    }
}
