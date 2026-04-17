using FluentValidation;
using SimpleStore.Application.Dto.Category;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Validators;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Categories
{
    public record CreateCategoryCommand(string Name);
    public class CreateCategoryHandler
    {
        public readonly ICategoryRepository _repository;

        public CreateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateCategoryCommandValidator();
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

            var category = new Category
            {
                Name = command.Name
            };

            return await _repository.AddAsync(category, cancellationToken);
        }
    }
}
