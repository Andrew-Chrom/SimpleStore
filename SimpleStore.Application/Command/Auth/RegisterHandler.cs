using Microsoft.AspNetCore.Identity;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Command.Auth
{
    public record RegisterCommand(string Email, string Password);
    public class RegisterHandler
    {
        private readonly UserManager<User> _userManager;

        public RegisterHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<Guid>> Handle(RegisterCommand cmd)
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
