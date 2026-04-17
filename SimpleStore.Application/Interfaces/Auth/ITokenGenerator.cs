using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface ITokenGenerator
    {
        string Generate(string secretKey, string issuer, string audience, double expiration, IEnumerable<Claim> claims=null);
    }
}
