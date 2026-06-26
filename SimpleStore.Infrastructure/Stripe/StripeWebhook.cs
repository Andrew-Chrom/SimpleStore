using Microsoft.Extensions.Options;
using SimpleStore.Application.Dto.Orders;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Domain.Enum;
using SimpleStore.Infrastructure.Options;
using Stripe;
using Stripe.Checkout;

namespace SimpleStore.Infrastructure.Stripe
{
    public class StripeWebhook : IWebhookParser
    {
        private readonly StripeSettings _stripeSettings;
        public StripeWebhook(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        public async Task<PaymentEvent> ParseEventAsync(string json, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _stripeSettings.WebhookSecret,
                throwOnApiVersionMismatch: false
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                if (session == null)
                {
                    return new PaymentEvent { Event = PaymentEventEnum.PaymentFailed, OrderId = Guid.Empty };
                }

                if (session.Metadata.TryGetValue("orderId", out var orderIdString) &&
                    Guid.TryParse(orderIdString, out var orderId))
                {
                    return new PaymentEvent
                    {
                        Event = PaymentEventEnum.PaymentSuccessful,
                        OrderId = orderId
                    };
                }
            }

            return new PaymentEvent
            {
                Event = PaymentEventEnum.PaymentFailed,
                OrderId = Guid.Empty
            };
        }
    }
}