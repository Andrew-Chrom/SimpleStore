using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Infrastructure.Repositories
{
    internal class ProductsReadonlyRepository : IProductsReadonlyRepository
    {
        public readonly QueryDbContext _db;

        public ProductsReadonlyRepository(QueryDbContext db)
        {
            _db = db;
        }
        public async Task<List<ProductListItemDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _db.Products
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    InStock = p.StockQuantity > 1,
                    CategoryName = p.Category.Name
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
