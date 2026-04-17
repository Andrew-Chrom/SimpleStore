using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Auth
{
    public class TokenIssuerService : ITokenIssuerService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ApplicationDbContext _context;
        public TokenIssuerService(IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService, ApplicationDbContext context)
        {
            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
            _context = context;
        }

        public async Task<AuthenticateResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
        {
            var refreshToken = _refreshTokenService.Generate(user);
            await _context.RefreshTokens.AddAsync(new RefreshToken { UserId = user.Id, Token = refreshToken }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new AuthenticateResponse
            {
                AccessToken = _accessTokenService.Generate(user),
                RefreshToken = refreshToken
            };
        }
    }
}
