using Microsoft.AspNetCore.Identity;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;


namespace SimpleStore.Application.Command.Auth
{
    public record ChangeRoleCommand(Guid UserId);
    public class ToggleAdminRoleHandler
    {
        public readonly UserManager<User> _userManager;

        public ToggleAdminRoleHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task Handle(ChangeRoleCommand cmd, CancellationToken ct) 
        {
            var user = await _userManager.FindByIdAsync(cmd.UserId.ToString());

            if (user == null)
                throw new Exception("User not found");

            var currentRole = (await _userManager.GetRolesAsync(user))[0];
            var newRole = currentRole == Roles.Admin ? Roles.Customer : Roles.Admin;

            await _userManager.RemoveFromRoleAsync(user, currentRole);
            await _userManager.AddToRoleAsync(user, newRole);
        }
    }
}
