using SimpleStore.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Products
{
    public record DeleteProductCommand(Guid Id);
    public class DeleteCategoryHandler
    {
        public readonly IProductsWritableRepository _repository;

        public DeleteCategoryHandler(IProductsWritableRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
