using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleStore.Application.Dto.Orders;
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
        ILogger<StripeService> _logger;
        public StripeService(IOptions<StripeSettings> stripeSettings,
            ILogger<StripeService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
        }
        public async Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(Entities.Order order, CancellationToken ct)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            _logger.LogInformation($"Order Id = {order.Id}");

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
                SuccessUrl = $"{_stripeSettings.SuccessUrl}?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_stripeSettings.CancelUrl}?sessionId={{CHECKOUT_SESSION_ID}}",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: ct);

            return new CheckoutSessionResponse(session.Url, session.Id, session.PaymentIntentId); ;
        }
    }
}