using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Products
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        string Barcode,
        int StockQuantity,
        Guid CategoryId);
    public class CreateProductHandler
    {
        private readonly IProductsWritableRepository _repository;

        public CreateProductHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateProductCommandValidator();
            var result = validator.Validate(command);

            if (!result.IsValid)
                return DomainErrors.Product.Validation;

            var product = new Product
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Barcode = command.Barcode,
                StockQuantity = command.StockQuantity,
                CategoryId = command.CategoryId
            };

            return await _repository.CreateAsync(product, cancellationToken);
        }
    }
}
