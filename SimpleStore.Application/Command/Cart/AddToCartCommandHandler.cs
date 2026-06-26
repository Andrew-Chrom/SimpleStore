using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Command.Cart
{
    public record AddToCartCommand(Guid UserId, Guid ProductId);
    public class AddToCartCommandHandler
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductsWritableRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddToCartCommandHandler(ICartRepository cartRepository,
            IProductsWritableRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(AddToCartCommand cmd, CancellationToken ct)
        {
            if (await _cartRepository.GetByIdAsync(cmd.UserId, cmd.ProductId, ct) != null)
            {
                return DomainErrors.Cart.Conflict;
            }

            var product = await _productRepository.GetByIdAsync(cmd.ProductId, ct);

            if (product == null)
                return DomainErrors.Product.NotFound;

            var cartItem = new CartItem { ProductId = cmd.ProductId, UserId = cmd.UserId, Quantity = 1 };
            var result = await _cartRepository.AddAsync(cartItem, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return result;

        }

    }
}
