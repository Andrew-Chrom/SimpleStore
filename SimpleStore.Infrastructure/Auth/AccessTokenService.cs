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
    public class AccessTokenService : IAccessTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        public readonly JwtSettings _jwtSettings;
        public AccessTokenService(ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings)
        {
            _tokenGenerator = tokenGenerator;
            _jwtSettings = jwtSettings.Value;
        }
        public string Generate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            return _tokenGenerator.Generate(_jwtSettings.AccessTokenSecret,
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                _jwtSettings.AccessTokenExpirationMinutes,
                claims);
        }
    }
}
