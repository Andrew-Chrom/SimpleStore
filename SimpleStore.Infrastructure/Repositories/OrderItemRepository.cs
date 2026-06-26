using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly CommandDbContext _ctx;

        public OrderItemRepository(CommandDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<OrderItem>> GetAllAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            return await _ctx.OrderItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
        public async Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _ctx.OrderItems.FirstOrDefaultAsync(o => o.Id == id, ct);
        }
        public async Task<Guid> AddAsync(OrderItem order, CancellationToken ct)
        {
            _ctx.OrderItems.Add(order);
            return order.Id;
        }
        public async Task<Guid> UpdateAsync(OrderItem order, CancellationToken ct)
        {
            _ctx.OrderItems.Update(order);
            return order.Id;
        }
        public async Task DeleteAsync(OrderItem order, CancellationToken ct)
        {
            _ctx.OrderItems.Remove(order);
        }
    }
}
