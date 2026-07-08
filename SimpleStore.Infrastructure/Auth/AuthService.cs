using Microsoft.AspNetCore.Identity;
using SimpleStore.Application.Command.Auth;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Application.Query.Auth;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimpleStore.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenIssuerService _authService;
        public AuthService(UserManager<User> userManager, ITokenIssuerService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }
        public async Task<Result<AuthenticateResponse>> LoginAsync(LoginQuery query, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(query.Email);

            if (user == null)
                return DomainErrors.Authentication.Unauthorized;

            if (await _userManager.CheckPasswordAsync(user, query.Password))
                return await _authService.IssueTokensAsync(user, ct);
            else
                return DomainErrors.Authentication.Unauthorized;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterCommand cmd, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(cmd.Email) is not null)
                return DomainErrors.Authentication.EmailExists;


            var user = new User
            {
                UserName = cmd.Email,
                Email = cmd.Email
            };

            var result = await _userManager.CreateAsync(user, cmd.Password);
            if (!result.Succeeded)
            {
                return DomainErrors.Authentication.IdentityError(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, Roles.Customer);
            return user.Id;
        }
    }
}
