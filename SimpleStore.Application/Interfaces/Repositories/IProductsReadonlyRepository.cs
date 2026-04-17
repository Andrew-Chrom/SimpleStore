using SimpleStore.API.Query.Products;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IProductsReadonlyRepository
    {
        Task<List<ProductListItemDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
