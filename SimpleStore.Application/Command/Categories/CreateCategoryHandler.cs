using FluentValidation;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Category;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Application.Validators;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace SimpleStore.Application.Command.Categories
{
    public record CreateCategoryCommand(string Name);
    public class CreateCategoryHandler
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCategoryHandler(ICategoryRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            //var validator = new CreateCategoryCommandValidator();
            //var result = validator.Validate(command);

            //if (!result.IsValid)
            //{
            //    return DomainErrors.Category.Validation;
            //}

            var category = new Category
            {
                Name = command.Name
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = await _repository.AddAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                scope.Complete(); // Кажемо EF зберегти все негайно
                return id;
            }

            //var id = await _repository.AddAsync(category, cancellationToken);
            //var changes = await _unitOfWork.SaveChangesAsync(cancellationToken);
            //Console.WriteLine($"SAVED CHANGES: {changes}");
            //return id;
        }
    }
}
