using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Infrastructure.Options;
using Stripe;
using Stripe.Checkout;

namespace SimpleStore.Domain.Stripe
{
    public class StripeService : IOrderService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderRepository _orderRepository;
        ILogger<StripeService> _logger;
        public StripeService(IOptions<StripeSettings> stripeSettings,
            IOrderRepository orderRepository,
            ILogger<StripeService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task<string> CreateCheckoutSessionAsync(Entities.Order order, CancellationToken ct)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            _logger.LogInformation($"{order.OrderItems}");

            var lineItems = order.OrderItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)(item.UnitPrice * 100), 
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://saleable-calceolate-carolyne.ngrok-free.dev/api/orders/success?sessionId={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://saleable-calceolate-carolyne.ngrok-free.dev/api/orders/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: ct);

            order.StripeSessionId = session.Id;
            order.StripePaymentIntentId = session.PaymentIntentId;
            await _orderRepository.UpdateAsync(order, ct);
            
            return session.Url;
        }
    }
}