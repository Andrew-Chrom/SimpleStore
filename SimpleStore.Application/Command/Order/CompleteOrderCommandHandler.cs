using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Order
{
    public record CompleteOrderCommand(Guid OrderId);
    public class CompleteOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductsWritableRepository _productRepository;
        public CompleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductsWritableRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(CompleteOrderCommand cmd, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(cmd.OrderId, ct);

            if (order is null)
                return DomainErrors.Order.NotFound;

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);

                if (product is null)
                    return DomainErrors.Product.NotFound;
                if (product.StockQuantity < item.Quantity)
                    return DomainErrors.Order.Conflict;

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product, ct);
            }

            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order, ct);
            return Result.Success();
        }

    }
}
