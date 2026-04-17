using FluentValidation;
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
        public readonly ICategoryRepository _repository;

        public UpdateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateCategoryCommandValidator();
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

            var category = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Product with ID {command.Id} not found.");
            }
            
            category.Name = command.Name;
            
            await _repository.UpdateAsync(category, cancellationToken);
        }
    }
}
