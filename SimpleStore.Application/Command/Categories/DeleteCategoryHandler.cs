using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Application.Command.Categories
{
    public record DeleteCategoryCommand(Guid Id);
    public class DeleteCategoryHandler
    {
        private readonly ICategoryRepository _repository;

        public DeleteCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (category == null)
                return DomainErrors.Category.NotFound;

            await _repository.DeleteAsync(category, cancellationToken);
            return Result.Success();
        }
    }
}
