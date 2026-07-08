using SimpleStore.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Services
{
    internal class EmailService : IEmailService
    {
        public Task SendOrderCompletedEmailAsync(Guid userId, Guid orderId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SendOrderCreatedEmailAsync(Guid userId, Guid orderId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SendOrderPaidEmailAsync(Guid userId, Guid orderId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
