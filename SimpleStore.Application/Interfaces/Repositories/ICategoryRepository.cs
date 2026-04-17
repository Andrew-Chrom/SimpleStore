using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> AddAsync(Category category, CancellationToken ct);
        Task<Guid> UpdateAsync(Category category, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
