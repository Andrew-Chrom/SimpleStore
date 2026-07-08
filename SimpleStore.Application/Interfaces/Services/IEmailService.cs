using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOrderCreatedEmailAsync(Guid userId, Guid orderId, CancellationToken ct);
        Task SendOrderPaidEmailAsync(Guid userId, Guid orderId, CancellationToken ct);
        Task SendOrderCompletedEmailAsync(Guid userId, Guid orderId, CancellationToken ct);

    }
}
