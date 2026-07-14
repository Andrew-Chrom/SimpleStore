using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CommandDbContext _ctx;

        public OrderRepository(CommandDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<Order>> GetAllAsync(Guid userId, int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            return await _ctx.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Where(o => o.UserId == userId)
                .ToListAsync(ct);
        }
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _ctx.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }
        public async Task<Guid> AddAsync(Order order, CancellationToken ct)
        {
            _ctx.Orders.Add(order);
            return order.Id;
        }
        public async Task<Guid> UpdateAsync(Order order, CancellationToken ct)
        {
            _ctx.Orders.Update(order);
            return order.Id;
        }
        public async Task DeleteAsync(Order order, CancellationToken ct)
        {
            _ctx.Orders.Remove(order);
        }
        public async Task<List<Order>> GetExpiredPendingOrdersAsync(DateTime expirationTime, CancellationToken ct)
        {
            return await _ctx.Orders
                .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt <= expirationTime)
                .ToListAsync(ct);
        }
    }
}
