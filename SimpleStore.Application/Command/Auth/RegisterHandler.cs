using Microsoft.AspNetCore.Identity;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task<IdentityResult> Handle(RegisterCommand cmd)
        {
            if (await _userManager.FindByEmailAsync(cmd.Email) is not null)
            {
                throw new Exception("User with this email exists");
            }

            var user = new User
            {
                UserName = cmd.Email,
                Email = cmd.Email
            };

            var result = await _userManager.CreateAsync(user, cmd.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Customer);
            }

            return result;
        }
    }
}
