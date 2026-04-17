using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Query.Categories
{
    public record GetCategoryByIdQuery(Guid Id);
    public class GetCategoryByIdHandler
    {
        public readonly ICategoryRepository _repository;
        public GetCategoryByIdHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Category> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {query.Id} not found.");
            }
            return category;
        }
    }
}
