using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Domain.Enum
{
    public enum PaymentEventEnum
    {
        PaymentInitiated,
        PaymentSuccessful,
        PaymentFailed,
        PaymentRefunded
    }
}
