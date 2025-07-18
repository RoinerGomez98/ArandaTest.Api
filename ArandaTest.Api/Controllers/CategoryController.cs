using ArandaTest.Application.DTOs;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArandaTest.Api.Controllers
{
    [Authorize]
    [Route("api/categories")]
    [ApiController]
    public class CategoryController(ICategoryService cat) : ControllerBase
    {
        private readonly ICategoryService _cat = cat;

        [HttpGet]
        public async Task<GenericResponse<PagedResultDto<CategoryDto>>> Categories(
     [FromQuery] string? name = null,
     [FromQuery] Domain.Enums.SortBy sortBy = Domain.Enums.SortBy.Name,
     [FromQuery] Domain.Enums.SortOrder sortOrder = Domain.Enums.SortOrder.Ascending,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                return new GenericResponse<PagedResultDto<CategoryDto>> { Result = null, Message = "La página debe ser mayor a 0", Status = System.Net.HttpStatusCode.BadRequest };

            if (pageSize < 1 || pageSize > 100)
                return new GenericResponse<PagedResultDto<CategoryDto>> { Result = null, Message = "El tamaño de página debe estar entre 1 y 100", Status = System.Net.HttpStatusCode.BadRequest };

            var filter = new CategoryFilterDto
            {
                Name = name,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize
            };
            return await _cat.GetAllAsync(filter);
        }

        [HttpGet("{id}")]
        public async Task<GenericResponse<CategoryDto>> CategoryById(Guid id)
        {
            var product = await _cat.GetByIdAsync(id);

            if (product == null)
                return new GenericResponse<CategoryDto> { Result = null, Message = $"Categoria con ID {id} no encontrado", Status = System.Net.HttpStatusCode.BadRequest };

            return product;
        }

        [HttpPost]
        public async Task<GenericResponse<int>> CreateCategory([FromBody] CategoryDto CategoryDto)
        {
            var result = await _cat.CreateAsync(CategoryDto);
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }

        [HttpPatch("{id}")]
        public async Task<GenericResponse<int>> UpdateCategoryt(Guid id, [FromBody] CategoryDto category)
        {
            var result = await _cat.UpdateAsync(id, category);
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }
    }
}
