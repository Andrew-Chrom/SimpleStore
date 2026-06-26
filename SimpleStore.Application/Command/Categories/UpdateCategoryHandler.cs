using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Application.Validators;

namespace SimpleStore.Application.Command.Categories
{

    public record UpdateCategoryCommand(Guid Id, string Name);
    public class UpdateCategoryHandler
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCategoryHandler(ICategoryRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
