using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Extensions;
using SimpleStore.Application.Command.Cart;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Cart;
using SimpleStore.Application.Query.Cart;
using System.Security.Claims;
using Wolverine;

namespace SimpleStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public readonly IMessageBus _bus;
        public CartController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bus.InvokeAsync<CartResponse>(new GetCartQuery(Guid.Parse(userId)));
            return Ok(result);
        }

        [HttpPost("{productId}")]
        public async Task<ActionResult<Guid>> AddToCart(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _bus.InvokeAsync<Result>(new AddToCartCommand(Guid.Parse(userId), productId));
            return result.ToActionResult();
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> RemoveFromCart(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _bus.InvokeAsync<Result>(new RemoveFromCartCommand(Guid.Parse(userId), productId));

            return result.ToActionResult();
        }

        [HttpPatch("{productId}")]
        public async Task<ActionResult> UpdateCartItemQuantity([FromRoute] Guid productId, [FromBody] ItemQuantityDto quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _bus.InvokeAsync<Result>(new UpdateCartItemQuantityCommand(Guid.Parse(userId), productId, quantity.Quantity));
            return result.ToActionResult();
        }


        //[HttpDelete]
        //public async Task<ActionResult> ClearCart()
        //{
        //    var userId = User.FindFirstValue("id");
        //    var result = await _bus.InvokeAsync<Result>(new ClearCartCommand(Guid.Parse(userId)));
        //    return result.ToActionResult();
        //}
    }
}
