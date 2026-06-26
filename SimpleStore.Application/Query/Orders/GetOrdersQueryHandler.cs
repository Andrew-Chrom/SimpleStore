
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Query.Orders
{
    public record GetOrdersQuery(Guid UserId, int Page, int PageSize); // need role for admins to get all orders
    public class GetOrdersQueryHandler
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<List<Order>> Handle(GetOrdersQuery query, CancellationToken ct)
        {
            return await _orderRepository.GetAllAsync(query.UserId, query.Page, query.PageSize, ct);
        }

    }
}
