using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Order
{
    public record PlaceOrderCommand(Guid UserId);
    public class PlaceOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductsReadonlyRepository _productRepository;
        private readonly IOrderService _orderService;
        public PlaceOrderCommandHandler(IOrderRepository orderRepository, 
            ICartRepository cartRepository,
            IOrderService orderService,
            IProductsReadonlyRepository productRepository,
            IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderService = orderService;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<string>> Handle(PlaceOrderCommand cmd, CancellationToken ct)
        {
            var cartItems = await _cartRepository.GetAllAsync(cmd.UserId, ct);
            var carts = cartItems.Items;

            if (carts is null)
                return DomainErrors.Cart.NotFound;

            var order = new Domain.Entities.Order
            {
                UserId = cmd.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
            var orderId = await _orderRepository.AddAsync(order, ct);

            foreach (var item in carts)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                await _orderItemRepository.AddAsync(orderItem, ct);
                await _cartRepository.DeleteAsync(item, ct);
            }
            var orderWithItems = await _orderRepository.GetByIdAsync(orderId, ct);
            var checkoutUrl = await _orderService.CreateCheckoutSessionAsync(orderWithItems, ct);
            return checkoutUrl;
        }
    }
}
