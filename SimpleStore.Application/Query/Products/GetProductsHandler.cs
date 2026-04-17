using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.API.Query.Products
{
    public record GetAllProductsQuery(int Page=0, int PageSize=25);
    public class GetProductsHandler
    {
        public readonly IProductsReadonlyRepository _repository;
        public GetProductsHandler(IProductsReadonlyRepository repository) 
        {
            _repository = repository;
        }
        public async Task<List<ProductListItemDto>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(query.Page, query.PageSize, cancellationToken);
        }
    }
}
