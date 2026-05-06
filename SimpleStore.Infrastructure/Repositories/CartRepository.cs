using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CartResponse> GetAllAsync(Guid userId, CancellationToken ct)
        {
            decimal totalPrice = _context.CartItems
                .Where(cartItem => cartItem.UserId == userId)
                .Sum(cartItem => cartItem.Quantity * cartItem.Product.Price);
            var cartItems = await _context.CartItems
                .Include(p => p.Product)
                .Where(cartItem => cartItem.UserId == userId)
                .ToListAsync(ct);
            return new CartResponse
            {
                Items = cartItems,
                TotalPrice = totalPrice
            };
        }

        public async Task<CartItem?> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(cartItem => cartItem.UserId == userId &&
                cartItem.ProductId == productId, ct);

            return cartItem;
        }

        public async Task UpdateAsync(CartItem cartItem, CancellationToken ct)
        {
            _context.CartItems.Update(cartItem);
        }

        public async Task<Guid> AddAsync(CartItem cartItem, CancellationToken ct)
        {
            await _context.CartItems.AddAsync(cartItem);

            return cartItem.Id;
        }
        public async Task DeleteAsync(CartItem cartItem, CancellationToken ct)
        {
            _context.CartItems.Remove(cartItem);
        }

        public async Task ClearCartAsync(Guid userId, CancellationToken ct)
        {
            var items = _context.CartItems.Where(x => x.UserId == userId);
            _context.CartItems.RemoveRange(items);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
