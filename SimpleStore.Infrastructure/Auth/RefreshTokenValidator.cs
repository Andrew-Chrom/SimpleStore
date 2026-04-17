using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Domain.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SimpleStore.Infrastructure.Auth
{
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
        private readonly ITokenGenerator _tokenGenerator;
        public readonly JwtSettings _jwtSettings;
        public RefreshTokenValidator(ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings)
        {
            _tokenGenerator = tokenGenerator;
            _jwtSettings = jwtSettings.Value;
        }
        public bool Validate(string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenSecret)),
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            try
            {
                jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters,
                    out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
