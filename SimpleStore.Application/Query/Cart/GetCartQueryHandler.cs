using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Query.Cart
{
    public record GetCartQuery(Guid UserId);
    public class GetCartQueryHandler
    {
        private readonly ICartRepository _repository;
        public GetCartQueryHandler(ICartRepository repository) 
        {
            _repository = repository;
        }

        public async Task<CartResponse> Handle(GetCartQuery query, CancellationToken ct)
        {
            return await _repository.GetAllAsync(query.UserId, ct);
        }
    }
}
