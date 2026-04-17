using Microsoft.AspNetCore.Identity;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Query.Auth
{
    public record LoginQuery(string Email, string Password);
    public class LoginHandler
    {

        private readonly UserManager<User> _userManager;
        private readonly ITokenIssuerService _authService;

        public LoginHandler(UserManager<User> userManager, ITokenIssuerService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }
        public async Task<AuthenticateResponse> Handle(LoginQuery query, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(query.Email);

            if (user == null)
                throw new Exception("Unathorized");

            if (await _userManager.CheckPasswordAsync(user, query.Password))
                return await _authService.IssueTokensAsync(user, default);
            else
                throw new Exception("Unathorized");
        }

    }
}
