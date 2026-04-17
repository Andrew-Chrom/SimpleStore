using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        //Task<List<>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<RefreshToken?> GetByIdAsync(string token, CancellationToken ct);
        //Task<Guid> AddAsync(string token, CancellationToken ct);
        //Task<Guid> UpdateAsync(Category category, CancellationToken ct);
        Task DeleteAsync(RefreshToken token, CancellationToken ct);
    }
}
