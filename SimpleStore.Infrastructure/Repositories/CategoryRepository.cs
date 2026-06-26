using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Category;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CommandDbContext _context;

        public CategoryRepository(CommandDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            return await _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
        public async Task<Category> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }
        public async Task<Guid> AddAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Add(category);
            return category.Id;
        }
        public async Task<Guid> UpdateAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Update(category);
            return category.Id;
        }
        public async Task DeleteAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Remove(category);
        }
    }
}
