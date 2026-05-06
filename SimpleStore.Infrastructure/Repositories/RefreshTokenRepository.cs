using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken?> GetByIdAsync(string token, CancellationToken ct)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
        }
        public async Task DeleteAsync(RefreshToken token, CancellationToken ct)
        { 
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync(ct);
        }
    }
}
