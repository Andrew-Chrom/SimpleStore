using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories
{
    public class ProductsWritableRepository : IProductsWritableRepository
    {
        private readonly CommandDbContext _db;
        public ProductsWritableRepository(CommandDbContext db)
        {
            _db = db;
        }
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
        {
            return await _db.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(ct);
        }
        public async Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken ct)
        {
            _db.Products.UpdateRange(products);
        }
        public async Task<Guid> CreateAsync(Product product, CancellationToken cancellationToken)
        { 
            _db.Products.Add(product);
            return product.Id;
        }
        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _db.Products.Update(product);
        }
        public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            _db.Products.Remove(product);           
        }
    }
}
