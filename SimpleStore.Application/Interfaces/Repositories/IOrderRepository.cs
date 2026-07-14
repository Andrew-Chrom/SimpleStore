using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync(Guid userId, int page, int pageSize, CancellationToken ct);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> AddAsync(Order order, CancellationToken ct);
        Task<Guid> UpdateAsync(Order order, CancellationToken ct);
        Task DeleteAsync(Order order, CancellationToken ct);
        Task<List<Order>> GetExpiredPendingOrdersAsync(DateTime expirationTime, CancellationToken ct);
    }
}
