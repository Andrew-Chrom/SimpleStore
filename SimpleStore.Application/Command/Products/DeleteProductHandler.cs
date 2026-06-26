using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;

namespace SimpleStore.Application.Command.Products
{
    public record DeleteProductCommand(Guid Id);
    public class DeleteProductHandler
    {
        private readonly IProductsWritableRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductHandler(IProductsWritableRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
                return DomainErrors.Product.NotFound;

            await _repository.DeleteAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
