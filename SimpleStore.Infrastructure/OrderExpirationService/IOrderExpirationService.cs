using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.OrderExpirationService
{
    public interface IOrderExpirationService
    {
        Task CancelExpiredOrdersAsync(CancellationToken ct);
    }
}
