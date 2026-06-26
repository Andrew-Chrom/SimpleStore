using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Order
{
    public record PlaceOrderCommand(Guid UserId);
    public class PlaceOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductsWritableRepository _productWritbaleRepository;
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlaceOrderCommandHandler> _logger;
        public PlaceOrderCommandHandler(IOrderRepository orderRepository, 
            ICartRepository cartRepository,
            IOrderService orderService,
            IProductsWritableRepository productWritbaleRepository,
            IUnitOfWork unitOfWork,
            ILogger<PlaceOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderService = orderService;
            _productWritbaleRepository = productWritbaleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(PlaceOrderCommand cmd, CancellationToken ct)
        {
            var cartItems = await _cartRepository.GetAllAsync(cmd.UserId, ct);
            var carts = cartItems.Items;

            if (carts is null || !carts.Any())
                return DomainErrors.Cart.NotFound;

            var order = new Domain.Entities.Order
            {
                UserId = cmd.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = carts.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    Product = item.Product 
                }).ToList()
            };

            var productIds = order.OrderItems.Select(i => i.ProductId).ToList();
            var products = await _productWritbaleRepository.GetByIdsAsync(productIds, ct);
            var productsDictionary = products.ToDictionary(p => p.Id);

            foreach (var item in order.OrderItems)
            {
                if (!productsDictionary.TryGetValue(item.ProductId, out var product))
                    return DomainErrors.Product.NotFound;

                if (product.StockQuantity < item.Quantity)
                    return DomainErrors.Order.Conflict;
                item.Product = product;
                product.StockQuantity -= item.Quantity;
            }
            await _productWritbaleRepository.UpdateRangeAsync(products, ct);


            await _orderRepository.AddAsync(order, ct);

            try
            {
                var savedRows = await _unitOfWork.SaveChangesAsync(ct);
                _logger.LogInformation($"Saved rows: {savedRows}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while placing order for user {UserId}. Someone else bought the product first.", cmd.UserId);

                return DomainErrors.Order.Conflict;
            }

            var stripeResponse = await _orderService.CreateCheckoutSessionAsync(order, ct);

            order.StripeSessionId = stripeResponse.SessionId;
            order.StripePaymentIntentId = stripeResponse.PaymentIntentId;

            await _cartRepository.ClearCartAsync(cmd.UserId, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return stripeResponse.Url;
        }
    }
}
