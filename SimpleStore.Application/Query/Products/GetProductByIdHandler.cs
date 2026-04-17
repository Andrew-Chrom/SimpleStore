using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Query.Products
{
    public record GetProductByIdQuery(Guid Id);
    public class GetCategoryByIdHandler
    {
        public readonly IProductsReadonlyRepository _repository;
        public GetCategoryByIdHandler(IProductsReadonlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {query.Id} not found.");
            }
            return product;
        }


    }
}
