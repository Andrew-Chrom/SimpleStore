using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure
{
    public class CommandDbContext : ApplicationDbContext, IUnitOfWork
    {  
        public CommandDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
