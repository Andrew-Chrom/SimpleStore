using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Dto.Wishlist;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<List<WishlistItemDto>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<WishlistItemDto> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct);
        Task<Guid> AddAsync(Guid userId, Guid productId, CancellationToken ct);
        Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct);
        Task ClearWishlistAsync(Guid userId, CancellationToken ct);
    }
}
