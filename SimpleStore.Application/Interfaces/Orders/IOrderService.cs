using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<string> CreateCheckoutSessionAsync(Order order, CancellationToken ct);
    }
}
