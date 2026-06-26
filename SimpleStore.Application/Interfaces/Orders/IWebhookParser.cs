using SimpleStore.Application.Dto.Orders;

namespace SimpleStore.Application.Interfaces.Orders
{
    public interface IWebhookParser
    {
        Task<PaymentEvent> ParseEventAsync(string json, string signature);
    }
}
