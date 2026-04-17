using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        public readonly IProductsWritableRepository _repository;

        public CreateProductHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateProductCommandValidator();
            var result = validator.Validate(command);

            if(!result.IsValid)
            {
                var error = "";
                foreach (var failure in result.Errors)
                {
                    error += failure + "\n";
                }

                throw new ValidationException(error);
            }

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
