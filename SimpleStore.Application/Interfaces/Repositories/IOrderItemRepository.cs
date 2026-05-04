using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> AddAsync(OrderItem orderItem, CancellationToken ct);
        Task<Guid> UpdateAsync(OrderItem orderItem, CancellationToken ct);
        Task DeleteAsync(OrderItem orderItem, CancellationToken ct);
    }
}
