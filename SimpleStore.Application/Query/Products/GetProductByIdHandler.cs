using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Query.Products
{
    public record GetProductByIdQuery(Guid Id);
    public class GetProductByIdHandler
    {
        private readonly IProductsReadonlyRepository _repository;
        public GetProductByIdHandler(IProductsReadonlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Product>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (product == null)
                return DomainErrors.Product.NotFound;
            
            return product;
        }


    }
}
