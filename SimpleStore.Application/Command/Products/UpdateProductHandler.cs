using FluentValidation;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;

namespace SimpleStore.Application.Command.Products
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string Barcode,
        int StockQuantity,
        Guid CategoryId);
    public class UpdateProductHandler
    {
        private readonly IProductsWritableRepository _repository;

        public UpdateProductHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductCommandValidator();
            var result = validator.Validate(command);

            if (!result.IsValid)
                return Result.Failure(DomainErrors.Product.Validation);
            

            var product = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
                return Result.Failure(DomainErrors.Product.NotFound);
            

            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Barcode = command.Barcode;
            product.StockQuantity = command.StockQuantity;
            product.CategoryId = command.CategoryId;

            await _repository.UpdateAsync(product, cancellationToken);
            return Result.Success();
        }
    }
}
