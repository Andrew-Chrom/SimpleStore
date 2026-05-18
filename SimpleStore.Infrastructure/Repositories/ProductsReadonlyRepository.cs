using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Entities;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Infrastructure.Repositories
{
    public class ProductsReadonlyRepository : IProductsReadonlyRepository
    {
        private readonly QueryDbContext _db;

        public ProductsReadonlyRepository(QueryDbContext db)
        {
            _db = db;
        }
        public async Task<List<ProductListItemDto>> GetAllAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            return await _db.Products
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    InStock = p.StockQuantity > 0,
                    CategoryName = p.Category.Name
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
        public async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

    }
}
