using FluentValidation;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Categories
{

    public record UpdateCategoryCommand(Guid Id, string Name);
    public class UpdateCategoryHandler
    {
        private readonly ICategoryRepository _repository;

        public UpdateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateCategoryCommandValidator();
            var result = validator.Validate(command);

            if (!result.IsValid)
            {
                return DomainErrors.Category.Validation;
            }

            var category = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (category == null)
            {
                return DomainErrors.Category.NotFound;
            }
            
            category.Name = command.Name;
            
            await _repository.UpdateAsync(category, cancellationToken);
            return Result.Success();
        }
    }
}
