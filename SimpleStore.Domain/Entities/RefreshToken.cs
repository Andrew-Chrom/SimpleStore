using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SimpleStore.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User;
    }
}
