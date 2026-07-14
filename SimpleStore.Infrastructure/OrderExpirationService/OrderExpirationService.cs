using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.OrderExpirationService
{
    public class OrderExpirationService : IOrderExpirationService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderExpirationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CancelExpiredOrdersAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var expirationTime = DateTime.UtcNow.AddMinutes(-1);

            var expiredOrders = await repository.GetExpiredPendingOrdersAsync(expirationTime, ct);

            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
                await repository.UpdateAsync(order, ct);
            }
            await unitOfWork.SaveChangesAsync();
            
            //_logger.LogInformation("Cancelled {Count} expired orders", expiredOrders.Count);
        }
    }
}
