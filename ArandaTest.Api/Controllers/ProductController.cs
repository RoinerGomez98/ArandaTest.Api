using System.Net;
using System.Security.Claims;
using ArandaTest.Application.DTOs;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArandaTest.Api.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController(IProductService prod) : ControllerBase
    {
        private readonly IProductService _prod = prod;

        [HttpGet]
        public async Task<GenericResponse<PagedResultDto<ProductDto>>> Products(
     [FromQuery] string? name = null,
     [FromQuery] string? description = null,
     [FromQuery] string? category = null,
     [FromQuery] Domain.Enums.SortBy sortBy = Domain.Enums.SortBy.Name,
     [FromQuery] Domain.Enums.SortOrder sortOrder = Domain.Enums.SortOrder.Ascending,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                return new GenericResponse<PagedResultDto<ProductDto>> { Result = null, Message = "La página debe ser mayor a 0", Status = HttpStatusCode.BadRequest };

            if (pageSize < 1 || pageSize > 100)
                return new GenericResponse<PagedResultDto<ProductDto>> { Result = null, Message = "El tamaño de página debe estar entre 1 y 100", Status = HttpStatusCode.BadRequest };

            var filter = new ProductFilterDto
            {
                Name = name,
                ShortDescription = description,
                Category = category,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize
            };
            return await _prod.GetAllAsync(filter);
        }

        [HttpGet("{id}")]
        public async Task<GenericResponse<ProductDto>> ProductsById(Guid id)
        {
            var product = await _prod.GetByIdAsync(id);

            if (product == null)
                return new GenericResponse<ProductDto> { Result = null, Message = $"Producto con ID {id} no encontrado", Status = System.Net.HttpStatusCode.BadRequest };

            return product;
        }

        [HttpPost]
        public async Task<GenericResponse<int>> CreateProduct([FromForm] ProductCreateDto productDto, IFormFile imageUrl)
        {

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageUrl.FileName)}";
            productDto.ImageUrl = $"/Images/{fileName}";
            if (imageUrl != null && imageUrl.Length > 0)
            {
                var mimeType = imageUrl.ContentType;
                var validMimeTypes = new[] { "image/jpeg", "image/png" };

                if (!validMimeTypes.Contains(mimeType))
                {
                    return new GenericResponse<int>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Tipo MIME no permitido.",
                        Result = 0
                    };
                }
            }
            else
            {
                return new GenericResponse<int>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Debe cargar una imagen.",
                    Result = 0
                };

            }

            productDto.CreatedBy = Guid.Parse(User.FindFirst(ClaimTypes.Authentication)?.Value!);
            var result = await _prod.CreateAsync(productDto);

            if (result.Result > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageUrl.CopyToAsync(stream);
            }
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }

        [HttpPatch("{id}")]
        public async Task<GenericResponse<int>> UpdateProduct(Guid id, [FromForm] ProducUpdateDto updateProductDto, IFormFile? imageUrl)
        {
            var result = await _prod.UpdateAsync(id, updateProductDto);

            if (imageUrl != null && imageUrl.Length > 0 && result.Result > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var find = await _prod.GetByIdAsync(id);
                var filePath = Path.Combine(folderPath, find.Result!.ImageUrl.Replace("/Images/", ""));
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageUrl.CopyToAsync(stream);
            }
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteProduct(Guid id)
        {
            var find = await _prod.GetByIdAsync(id);
            var result = await _prod.DeleteAsync(id);
            if (result.Result)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                var fullPath = Path.Combine(folderPath, find.Result!.ImageUrl.Replace("/Images/", ""));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }
    }
}
