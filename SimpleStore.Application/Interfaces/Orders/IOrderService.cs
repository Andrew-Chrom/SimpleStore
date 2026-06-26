using SimpleStore.Application.Dto.Orders;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(Order order, CancellationToken ct);
    }
}
