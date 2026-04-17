using Microsoft.AspNetCore.Identity;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Auth
{
    public record RefreshCommand(string RefreshToken, CancellationToken cancellationToken);
    public class RefreshHandler
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenIssuerService _authService;
        private readonly IRefreshTokenRepository _repository;
        private readonly IRefreshTokenValidator _refreshTokenValidator;
        public RefreshHandler(
        UserManager<User> userManager,
        ITokenIssuerService authService,
        IRefreshTokenRepository repository,
        IRefreshTokenValidator refreshTokenValidator)
        {
            _userManager = userManager;
            _authService = authService;
            _repository = repository;
            _refreshTokenValidator = refreshTokenValidator;
        }
        public async Task<AuthenticateResponse> Handle(RefreshCommand cmd, CancellationToken ct)
        {
            var isValid = _refreshTokenValidator.Validate(cmd.RefreshToken);
            if (!isValid) throw new Exception("Unathorized");

            var refreshToken = await _repository.GetByIdAsync(cmd.RefreshToken,ct);

            if (refreshToken == null) throw new Exception("Unathorized");

            await _repository.DeleteAsync(refreshToken, ct);

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null) throw new Exception("Unathorized");

            var response = await _authService.IssueTokensAsync(user, ct);

            return response;
        }
    }
}
