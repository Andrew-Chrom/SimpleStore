using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStore.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
