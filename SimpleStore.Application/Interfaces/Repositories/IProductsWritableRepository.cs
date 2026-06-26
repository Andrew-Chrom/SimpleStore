using SimpleStore.Application.Dto.Product;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IProductsWritableRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
        Task<Guid> CreateAsync(Product product, CancellationToken cancellationToken);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken ct);
        Task DeleteAsync(Product product, CancellationToken cancellationToken);
    }
}
