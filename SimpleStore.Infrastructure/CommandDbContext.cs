using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure
{
    public class CommandDbContext : ApplicationDbContext
    {  
        public CommandDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
