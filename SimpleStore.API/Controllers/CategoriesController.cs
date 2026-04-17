using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Query.Categories;
using SimpleStore.Application.Query.Categories;
using SimpleStore.Domain.Entities;
using System.Xml.Linq;
using Wolverine;
using SimpleStore.Application.Dto.Category;
using SimpleStore.Application.Command.Categories;

namespace SimpleStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public readonly IMessageBus _bus;
        public CategoriesController(IMessageBus bus)
        {
            _bus = bus;
        }


        [HttpGet]
        public async Task<List<Category>> GetCategories([FromQuery] int page, [FromQuery] int pageSize)
        {
            return await _bus.InvokeAsync<List<Category>>(new GetAllCategoriesQuery(page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<Category> GetProductById(Guid id)
        {
            return await _bus.InvokeAsync<Category>(new GetCategoryByIdQuery(id));
        }

        [HttpPost]
        public async Task<Guid> CreateProduct([FromBody] СreateUpdateCategoryDto dto)
        {
            return await _bus.InvokeAsync<Guid>(new CreateCategoryCommand(dto.Name));
        }

        [HttpPut("{id}")]
        public async Task UpdateProduct(Guid id, [FromBody] СreateUpdateCategoryDto dto)
        {
            await _bus.InvokeAsync(new UpdateCategoryCommand(id, dto.Name));
        }

        [HttpDelete("{id}")]
        public async Task DeleteProduct(Guid id)
        {
            await _bus.InvokeAsync(new DeleteCategoryCommand(id));
        }

    }
}
