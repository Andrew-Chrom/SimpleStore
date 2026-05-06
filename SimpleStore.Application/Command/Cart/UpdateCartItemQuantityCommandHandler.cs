using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Cart
{
    public record UpdateCartItemQuantityCommand(Guid UserId, Guid ProductId, int Quantity);
    public class UpdateCartItemQuantityCommandHandler
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductsWritableRepository _productRepository;

        public UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository, 
            IProductsWritableRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(UpdateCartItemQuantityCommand command, CancellationToken ct)
        {
            var cartItem = await _cartRepository.GetByIdAsync(command.UserId, command.ProductId, ct);

            if (cartItem == null)
            {
                return DomainErrors.Cart.NotFound;
            }

            var product = await _productRepository.GetByIdAsync(command.ProductId, ct);

            if (product == null)
                return DomainErrors.Product.NotFound;

            if (command.Quantity > product.StockQuantity)
                return DomainErrors.Cart.Conflict;

            cartItem.Quantity = command.Quantity;
            await _cartRepository.UpdateAsync(cartItem, ct);

            await _cartRepository.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
