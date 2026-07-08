using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Extensions;
using SimpleStore.Application.Command.Auth;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Query.Auth;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure.Auth;
using Wolverine;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimpleStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private UserManager<User> _userManager;
        private ITokenIssuerService _authService;
        private readonly IRefreshTokenRepository _repository;
        private readonly IRefreshTokenValidator _refreshTokenValidator;
        public AuthController(IMessageBus bus, UserManager<User> userManager, ITokenIssuerService authService)
        {
            _bus = bus;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest model)
        {
            Result final_result;
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                final_result = DomainErrors.Authentication.EmailExists;
                return final_result.ToActionResult();
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                final_result = DomainErrors.Authentication.IdentityError(result.Errors);
                return final_result.ToActionResult();
            }

            await _userManager.AddToRoleAsync(user, Roles.Customer);
            return user.Id;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticateResponse>> Login([FromBody] LoginRequest model, CancellationToken ct)
        {
            Result<AuthenticateResponse> result;
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                result = DomainErrors.Authentication.Unauthorized;
            
            if (await _userManager.CheckPasswordAsync(user, model.Password))
                result = await _authService.IssueTokensAsync(user, ct);
            else
                result = DomainErrors.Authentication.Unauthorized;

            return result.ToActionResult();
        }
        

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticateResponse>> Refresh([FromBody] RefreshRequest model, CancellationToken cancellationToken)
        {
            Result<AuthenticateResponse> result;
            var isValid = _refreshTokenValidator.Validate(model.RefreshToken);
            if (!isValid) result = DomainErrors.Authentication.Unauthorized;

            var refreshToken = await _repository.GetByIdAsync(model.RefreshToken, cancellationToken);

            if (refreshToken == null) result = DomainErrors.Authentication.Unauthorized;
            await _repository.DeleteAsync(refreshToken, cancellationToken);

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null) result = DomainErrors.Authentication.Unauthorized;

            var response = await _authService.IssueTokensAsync(user, cancellationToken);
            result = response;

            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("role/{userId}")]
        public async Task<ActionResult> ToggleAdminRole ([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            Result result;
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                result = DomainErrors.Authentication.NotFound;
            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();
            var newRole = currentRole == Roles.Admin ? Roles.Customer : Roles.Admin;

            await _userManager.RemoveFromRoleAsync(user, currentRole);
            await _userManager.AddToRoleAsync(user, newRole);
            result = Result.Success();

            return result.ToActionResult();
        }

    }
}
