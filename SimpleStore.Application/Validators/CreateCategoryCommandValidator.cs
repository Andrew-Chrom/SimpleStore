using FluentValidation;
using SimpleStore.Application.Command.Categories;
using SimpleStore.Application.Dto.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Validators
{
    internal class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
        }
    }
}
