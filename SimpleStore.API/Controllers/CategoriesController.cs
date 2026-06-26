using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.API.Extensions;
using SimpleStore.API.Query.Categories;
using SimpleStore.Application.Command.Categories;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Category;
using SimpleStore.Application.Query.Categories;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Entities;
using Wolverine;

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
        public async Task<ActionResult<List<Category>>> GetCategories([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = await _bus.InvokeAsync<List<Category>>(new GetAllCategoriesQuery(page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetProductById(Guid id)
        {
            var result = await _bus.InvokeAsync<Result<Category>>(new GetCategoryByIdQuery(id));
            return result.ToActionResult();
        }


        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] СreateUpdateCategoryDto dto)
        {
            var result = await _bus.InvokeAsync<Result<Guid>>(new CreateCategoryCommand(dto.Name));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(Guid id, [FromBody] СreateUpdateCategoryDto dto)
        {
            var result = await _bus.InvokeAsync<Result>(new UpdateCategoryCommand(id, dto.Name));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var result = await _bus.InvokeAsync<Result>(new DeleteCategoryCommand(id));
            return result.ToActionResult();
        }

    }
}
