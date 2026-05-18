using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleStore.API.Extensions;
using SimpleStore.Application.Command.Order;
using SimpleStore.Application.Common;
using SimpleStore.Application.Query.Orders;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure.Options;
using System.Security.Claims;
using Wolverine;

namespace SimpleStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMessageBus _bus;
        public OrdersController(IOptions<StripeSettings> stripeSettings, IMessageBus bus)
        {
            _bus = bus;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var orders = await _bus.InvokeAsync<List<Order>>(new GetOrdersQuery(Guid.Parse(userId), page, pageSize));
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _bus.InvokeAsync<Result<string>>(new PlaceOrderCommand(Guid.Parse(userId)));
            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            var result = await _bus.InvokeAsync<Result>(new ProcessStripeWebhookCommand(json, signature));
            
            return result.ToActionResult();
        }

        [HttpGet("success")]
        public async Task<IActionResult> success([FromQuery] string sessionId)
        {
            return Ok($"Order {sessionId} completed!");
        }
        [HttpGet("failed")]
        public async Task<IActionResult> success()
        {
            return Ok("Order failed!");
        }


    }
}
