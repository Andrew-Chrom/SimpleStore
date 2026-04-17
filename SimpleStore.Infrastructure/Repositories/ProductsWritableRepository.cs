using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Repositories
{
    public class ProductsWritableRepository : IProductsWritableRepository
    {
        public readonly CommandDbContext _db;
        public ProductsWritableRepository(CommandDbContext db)
        {
            _db = db;
        }
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Guid> CreateAsync(Product product, CancellationToken cancellationToken)
        { 
            _db.Products.Add(product);
            await _db.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _db.Products.FindAsync(id);
            
            if(product == null)
                throw new KeyNotFoundException($"Product with id {id} not found.");

            _db.Products.Remove(product);
            await _db.SaveChangesAsync(cancellationToken);            
        }
    }
}
