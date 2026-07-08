using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Services
{
    public interface IOrderExpirationService
    {
        Task CancelExpiredOrdersAsync(CancellationToken ct);
    }
}
