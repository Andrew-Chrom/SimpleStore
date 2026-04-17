using SimpleStore.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Categories
{
    public record DeleteCategoryCommand(Guid Id);
    public class DeleteCategoryHandler
    {
        public readonly ICategoryRepository _repository;

        public DeleteCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
