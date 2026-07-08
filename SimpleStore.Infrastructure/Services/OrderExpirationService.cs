//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using SimpleStore.Application.Interfaces.Repositories;
//using SimpleStore.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SimpleStore.Infrastructure.Services
//{
//    public class OrderExpirationService : IOrderExpirationService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly ILogger<OrderExpirationService> _logger;
//        public OrderExpirationService(IServiceScopeFactory scopeFactory, ILogger<OrderExpirationService> logger)
//        {
//            _scopeFactory = scopeFactory;
//            _logger = logger;

//        }

//        public async Task CancelExpiredOrdersAsync(CancellationToken ct)
//        {
//            _logger.LogDebug("--> OrderExpirationService: Canceling expired orders");
            
//            using var scope = _scopeFactory.CreateScope();
//            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

//            var expiredOrders = await repository.GetExpiredOrdersAsync(ct);

//            foreach(var order in expiredOrders)
//            {
//                order.Status = OrderStatus.Cancelled;
//            }

//            _logger.LogDebug("--> OrderExpirationService: Finished canceling expired orders");

//            return;
//        }
//    }
//}
