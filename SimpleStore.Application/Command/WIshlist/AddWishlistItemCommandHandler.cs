
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

        public async Task<Guid> Handle(AddWishlistItemCommand cmd, CancellationToken ct)
        {
            var id = await _repository.AddAsync(cmd.UserId, cmd.ProductId, ct); 
            await _repository.SaveChangesAsync(ct);
            return id; 
        }
    }
}
