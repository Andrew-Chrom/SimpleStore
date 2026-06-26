
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;

namespace SimpleStore.Application.Command.WIshlist
{
    public record RemoveWishlistItemCommand(Guid UserId, Guid ProductId);
    public class RemoveWishlistItemCommandHandler
    {
        private readonly IWishlistRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public RemoveWishlistItemCommandHandler(IWishlistRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveWishlistItemCommand cmd, CancellationToken ct)
        {
            if (await _repository.GetByIdAsync(cmd.UserId, cmd.ProductId, ct) is null)
                return DomainErrors.Wishlist.Conflict;
                
            await _repository.RemoveAsync(cmd.UserId, cmd.ProductId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
