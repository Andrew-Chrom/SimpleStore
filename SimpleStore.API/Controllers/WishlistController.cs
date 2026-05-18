using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.Application.Command.WIshlist;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Wishlist;
using SimpleStore.Application.Query.Wishlist;
using System.Security.Claims;
using Wolverine;

namespace SimpleStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IMessageBus _bus;
        public WishlistController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<List<WishlistItemDto>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlist = await _bus.InvokeAsync<Result<List<WishlistItemDto>>>(new GetWishListQuery(Guid.Parse(userId)));
            return wishlist.Value;
        }

        [HttpPost("{productId}")]
        public async Task<Guid> AddToWishlist(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bus.InvokeAsync<Guid>(new AddWishlistItemCommand(Guid.Parse(userId), productId));
            return result;

        }

        [HttpDelete("{productId}")]
        public async Task RemoveFromWishlist(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bus.InvokeAsync<Result>(new RemoveWishlistItemCommand(Guid.Parse(userId), productId));
            return;
        }
    }
}
