using Microsoft.Extensions.Options;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Domain.Entities;
using SimpleStore.Domain.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SimpleStore.Infrastructure.Auth
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        public readonly JwtSettings _jwtSettings;
        public RefreshTokenService(ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings)
        {
            _tokenGenerator = tokenGenerator;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<string> GenerateAsync(User user)
        {
            return _tokenGenerator.Generate(_jwtSettings.RefreshTokenSecret,
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                _jwtSettings.AccessTokenExpirationMinutes);
        }
    }
}
