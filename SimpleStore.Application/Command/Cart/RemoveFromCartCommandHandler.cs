using Microsoft.EntityFrameworkCore;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SimpleStore.Application.Command.Cart
{
    public record RemoveFromCartCommand(Guid UserId, Guid ProductId);
    public class RemoveFromCartCommandHandler
    {
        private readonly ICartRepository _repository;

        public RemoveFromCartCommandHandler(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(RemoveFromCartCommand cmd, CancellationToken ct)
        {
            var cartItem = await _repository.GetByIdAsync(cmd.UserId, cmd.ProductId, ct);
            if (cartItem == null)
            {
                return DomainErrors.Cart.NotFound;
            }

            await _repository.DeleteAsync(cartItem, ct);
            await _repository.SaveChangesAsync(ct);
            return Result.Success();
        }

    }
}
