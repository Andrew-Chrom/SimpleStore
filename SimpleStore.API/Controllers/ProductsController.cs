using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Query.Products;
using SimpleStore.Application.Command.Products;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Query.Products;
using SimpleStore.Domain.Entities;
using System.Xml.Linq;
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
        public async Task<List<ProductListItemDto>> GetProducts([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            return await _bus.InvokeAsync<List<ProductListItemDto>>(new GetAllProductsQuery(page, pageSize), ct);
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(Guid id, CancellationToken ct)
        {
            return await _bus.InvokeAsync<Product>(new GetProductByIdQuery(id), ct);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<Guid> CreateProduct([FromBody] CreateUpdateProductDto dto, CancellationToken ct)
        {
            return await _bus.InvokeAsync<Guid>(new CreateProductCommand(
                                    dto.Name,
                                    dto.Description,
                                    dto.Price,
                                    dto.Barcode,
                                    dto.StockQuantity,
                                    dto.CategoryId), ct);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task UpdateProduct(Guid id, [FromBody] CreateUpdateProductDto dto)
        {
            await _bus.InvokeAsync(new UpdateProductCommand(
                                    id,
                                    dto.Name,
                                    dto.Description,
                                    dto.Price,
                                    dto.Barcode,
                                    dto.StockQuantity,
                                    dto.CategoryId));
        }

        [HttpDelete("{id}")]
        public async Task DeleteProduct(Guid id)
        {
            await _bus.InvokeAsync(new DeleteProductCommand(id));
        }

    }
}
