using Microsoft.IdentityModel.Tokens;
using SimpleStore.Application.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleStore.Infrastructure.Auth
{
    public class TokenGenerator : ITokenGenerator
    {
        public string Generate(string secretKey, string issuer, string audience, double expiration, IEnumerable<Claim> claims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer, audience, claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(expiration),
                creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
