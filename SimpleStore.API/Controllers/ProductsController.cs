using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Extensions;
using SimpleStore.API.Query.Products;
using SimpleStore.Application.Command.Products;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Query.Products;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;
using Wolverine;

namespace SimpleStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IMessageBus _bus;
        public ProductsController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductListItemDto>>> GetProducts([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _bus.InvokeAsync<List<ProductListItemDto>>(new GetAllProductsQuery(page, pageSize), ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(Guid id, CancellationToken ct)
        {
            var result = await _bus.InvokeAsync<Result<Product>>(new GetProductByIdQuery(id), ct);

            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateUpdateProductDto dto, CancellationToken ct)
        {
            var result = await _bus.InvokeAsync<Result<Guid>>(new CreateProductCommand(
                                    dto.Name,
                                    dto.Description,
                                    dto.Price,
                                    dto.Barcode,
                                    dto.StockQuantity,
                                    dto.CategoryId), ct);

            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(Guid id, [FromBody] CreateUpdateProductDto dto)
        {
            var result = await _bus.InvokeAsync<Result>(new UpdateProductCommand(
                                    id,
                                    dto.Name,
                                    dto.Description,
                                    dto.Price,
                                    dto.Barcode,
                                    dto.StockQuantity,
                                    dto.CategoryId));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var result = await _bus.InvokeAsync<Result>(new DeleteProductCommand(id));
            return result.ToActionResult();
        }

    }
}
