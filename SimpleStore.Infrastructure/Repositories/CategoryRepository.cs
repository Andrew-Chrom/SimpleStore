using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.Repositories;

namespace SimpleStore.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return await _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Guid> AddAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);
            return category.Id;
        }
        public async Task<Guid> UpdateAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(ct);
            return category.Id;
        }
        public async Task DeleteAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(ct);
        }
    }
}
