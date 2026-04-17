using FluentValidation;
using SimpleStore.Application.Command.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Validators
{
    internal class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name is required.")
                    .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
        }

    }
}
