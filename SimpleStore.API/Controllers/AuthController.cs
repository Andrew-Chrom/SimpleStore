using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Extensions;
using SimpleStore.Application.Command.Auth;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Query.Auth;
using SimpleStore.Domain.Constants;
using Wolverine;

namespace SimpleStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMessageBus _bus;
        public AuthController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest model)
        {

            var result = await _bus.InvokeAsync<Result<Guid>>(new RegisterCommand(model.Email, model.Password));

            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticateResponse>> Login([FromBody] LoginRequest model, CancellationToken ct)
        {
            var result = await _bus.InvokeAsync<Result<AuthenticateResponse>>(new LoginQuery(model.Email, model.Password), ct);
            return result.ToActionResult();
        }
        

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticateResponse>> Refresh([FromBody] RefreshRequest model, CancellationToken cancellationToken)
        {
            var result = await _bus.InvokeAsync<Result<AuthenticateResponse>>(new RefreshCommand(model.RefreshToken, cancellationToken));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("role/{userId}")]
        public async Task<ActionResult> ToggleAdminRole ([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var result = await _bus.InvokeAsync<Result>(new ChangeRoleCommand(userId));
            return result.ToActionResult();
        }

    }
}
