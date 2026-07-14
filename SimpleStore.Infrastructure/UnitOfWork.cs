using SimpleStore.Application.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommandDbContext _context;
        public UnitOfWork(CommandDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
