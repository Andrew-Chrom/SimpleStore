using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Domain.Constants;
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
        private readonly UserManager<User> _userManager;
        public readonly JwtSettings _jwtSettings;
        
        public AccessTokenService(ITokenGenerator tokenGenerator, 
                IOptions<JwtSettings> jwtSettings,
                UserManager<User> userManager)
        {
            _tokenGenerator = tokenGenerator;
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }
        public async Task<string> GenerateAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? Roles.Customer)
            };
            return _tokenGenerator.Generate(_jwtSettings.AccessTokenSecret,
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                _jwtSettings.AccessTokenExpirationMinutes,
                claims);
        }
    }
}
