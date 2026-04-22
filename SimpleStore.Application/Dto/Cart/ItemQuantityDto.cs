using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Dto.Cart
{
    public record ItemQuantityDto
    {
        public int Quantity { get; init; }
    }
}