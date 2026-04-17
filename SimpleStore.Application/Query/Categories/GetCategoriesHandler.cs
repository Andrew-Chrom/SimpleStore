using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.API.Query.Categories
{
    public record GetAllCategoriesQuery(int Page=0, int PageSize=25);
    public class GetCategoriesHandler
    {
        public readonly ICategoryRepository _repository;
        public GetCategoriesHandler(ICategoryRepository repository) 
        {
            _repository = repository;
        }

        public async Task<List<Category>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(query.Page, query.PageSize, cancellationToken);
        }
    }
}
