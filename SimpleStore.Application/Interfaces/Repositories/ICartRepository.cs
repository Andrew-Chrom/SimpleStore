using SimpleStore.Application.Dto.Cart;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<CartResponse> GetAllAsync(Guid userId, CancellationToken ct);
        Task<CartItem?> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct);
        Task UpdateAsync(CartItem cartItem, CancellationToken ct);
        Task<Guid> AddAsync(CartItem cartItem, CancellationToken ct);
        Task DeleteAsync(CartItem cartItem, CancellationToken ct);
        Task ClearCartAsync(Guid userId, CancellationToken ct);
    }
}
