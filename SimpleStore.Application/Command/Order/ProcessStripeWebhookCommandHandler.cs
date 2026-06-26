using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Domain.Enum;
using Wolverine;

namespace SimpleStore.Application.Command.Order
{
    public record ProcessStripeWebhookCommand(string Json, string Signature);
    public class ProcessStripeWebhookCommandHandler
    {
        private readonly IWebhookParser _parser;
        private readonly IMessageBus _bus;

        public ProcessStripeWebhookCommandHandler(IWebhookParser parser, IMessageBus bus)
        {
            _parser = parser;
            _bus = bus;
        }

        public async Task<Result> Handle(ProcessStripeWebhookCommand cmd, CancellationToken ct)
        {
            var paymentEvent = await _parser.ParseEventAsync(cmd.Json, cmd.Signature);

            if (paymentEvent == null)
                return DomainErrors.Payment.InvalidWebhook;


            switch (paymentEvent.Event)
            {
                case PaymentEventEnum.PaymentSuccessful:
                    await _bus.InvokeAsync(new CompleteOrderCommand(paymentEvent.OrderId));
                    return Result.Success();
                case PaymentEventEnum.PaymentFailed:
                    return DomainErrors.Payment.PaymentFailed;
                default:
                    return DomainErrors.Payment.InvalidWebhook;
            }
        }
    }
}
