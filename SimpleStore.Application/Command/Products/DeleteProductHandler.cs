using SimpleStore.Application.Common;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Errors;

namespace SimpleStore.Application.Command.Products
{
    public record DeleteProductCommand(Guid Id);
    public class DeleteProductHandler
    {
        private readonly IProductsWritableRepository _repository;

        public DeleteProductHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
                return DomainErrors.Product.NotFound;

            await _repository.DeleteAsync(product, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
