using FluentValidation;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;
using System;
using System.Collections.Generic;
using System.Text;

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
        public readonly IProductsWritableRepository _repository;

        public UpdateProductHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductCommandValidator();
            var result = validator.Validate(command);

            if (!result.IsValid)
            {
                var error = "";
                foreach (var failure in result.Errors)
                {
                    error += failure + "\n";
                }

                throw new ValidationException(error);
            }

            var product = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {command.Id} not found.");
            }

            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Barcode = command.Barcode;
            product.StockQuantity = command.StockQuantity;
            product.CategoryId = command.CategoryId;

            await _repository.UpdateAsync(product, cancellationToken);
        }
    }
}
