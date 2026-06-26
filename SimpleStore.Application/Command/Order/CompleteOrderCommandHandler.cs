using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Order
{
    public record CompleteOrderCommand(Guid OrderId);
    public class CompleteOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CompleteOrderCommandHandler(
        IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CompleteOrderCommand cmd, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(cmd.OrderId, ct);

            if (order is null)
                return DomainErrors.Order.NotFound;

            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }

    }
}
