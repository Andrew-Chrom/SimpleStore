using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Options
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }  
        public string WebhookSecret { get; set; }
    }
}
