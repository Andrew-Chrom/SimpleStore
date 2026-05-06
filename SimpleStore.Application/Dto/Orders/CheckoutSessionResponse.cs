using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Dto.Orders
{
    public record CheckoutSessionResponse(
        string Url,
        string SessionId,
        string PaymentIntentId
    );
}
