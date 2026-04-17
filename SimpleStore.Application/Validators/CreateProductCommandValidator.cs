using FluentValidation;
using SimpleStore.Application.Command.Products;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SimpleStore.Application.Validators
{
    internal class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(cmd => cmd.Name)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(cmd => cmd.Description)
                .MaximumLength(500).WithMessage("Description must be less than 500 charecters long");
            RuleFor(cmd => cmd.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");
            RuleFor(cmd => cmd.Barcode)
                .NotEmpty().WithMessage("Barcode is required.")
                .Matches(@"^\d{10}$").WithMessage("Barcode must be a 10-digit number.");
            RuleFor(cmd => cmd.StockQuantity)
                .NotEmpty().WithMessage("Stock quantity is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity should be greater or equal to 0.");
            RuleFor(cmd => cmd.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.");
        }
    }
}
