using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Dto.Wishlist;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly CommandDbContext _context;

        public WishlistRepository(CommandDbContext context)
        {
            _context = context;
        }
        public async Task<List<WishlistItemDto>> GetAllAsync(Guid userId, CancellationToken ct)
        {
            return await _context.WishlistItems.Where(x => x.UserId == userId)
                .Select(x => new WishlistItemDto
                {
                    ProductId = x.ProductId,
                    Name = x.Product.Name
                }).ToListAsync(ct);
        }


        public async Task<WishlistItemDto> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct)
        {
            return await _context.WishlistItems.Where(x => x.UserId == userId && x.ProductId == productId)
                .Select(x => new WishlistItemDto
                {
                    ProductId = x.ProductId,
                    Name = x.Product.Name
                }).FirstOrDefaultAsync(ct);
        }

        public async Task<Guid> AddAsync(Guid userId, Guid productId, CancellationToken ct)
        {
            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };
            await _context.WishlistItems.AddAsync(wishlistItem, ct);

            return wishlistItem.Id;
        }
        public async Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, ct);
            _context.WishlistItems.Remove(wishlistItem);
        }

        public async Task ClearWishlistAsync(Guid userId, CancellationToken ct)
        {
            var items = _context.WishlistItems.Where(x => x.UserId == userId);
            _context.WishlistItems.RemoveRange(items);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
