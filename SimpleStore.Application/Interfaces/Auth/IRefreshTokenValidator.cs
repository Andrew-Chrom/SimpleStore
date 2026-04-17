using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface IRefreshTokenValidator
    {
        bool Validate(string refreshToken);
    }
}
