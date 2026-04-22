using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
            List<CartItem> cartItems = await _context.CartItems
                .Where(cartItem => cartItem.UserId == userId)
                .ToListAsync();
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
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Guid> AddAsync(CartItem cartItem, CancellationToken ct)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync(ct);

            return cartItem.Id;
        }
        public async Task DeleteAsync(CartItem cartItem, CancellationToken ct)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(ct);
        }
    }
}
