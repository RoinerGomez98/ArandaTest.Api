using System.Net;
using System.Reflection;
using ArandaTest.Application.DTOs;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Entities;
using ArandaTest.Domain.Enums;
using ArandaTest.Domain.Interfaces;
using ArandaTest.Domain.Utils;
using AutoMapper;

namespace ArandaTest.Application.Implementations
{
    public class ProductService(IAppRepository<Products> repo, IMapper mapper) : IProductService
    {
        private readonly IAppRepository<Products> _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<GenericResponse<ProductDto>> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id, true);
            if (product == null)
            {

                return new GenericResponse<ProductDto>
                {
                    Result = null,
                    Message = "Producto no encontrado o inactivo",
                    Status = HttpStatusCode.NotFound
                };
            }

            GenericResponse<ProductDto> response = new()
            {
                Result = _mapper.Map<ProductDto>(product),
                Status = HttpStatusCode.OK
            };
            return response;
        }

        public async Task<GenericResponse<PagedResultDto<ProductDto>>> GetAllAsync(ProductFilterDto filter)
        {
            IQueryable<Products> filterQuery(IQueryable<Products> query)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(p => p.Name!.Contains(filter.Name));

                if (!string.IsNullOrWhiteSpace(filter.ShortDescription))
                    query = query.Where(p => p.ShortDescription != null && p.ShortDescription.Contains(filter.ShortDescription));

                if (!string.IsNullOrWhiteSpace(filter.Category))
                    query = query.Where(p => p.CategoryId == Guid.Parse(filter.Category));

                //query = query.Where(x => x.Status);
                return query;
            }

            IOrderedQueryable<Products> sortQuery(IQueryable<Products> query)
            {
                return filter.SortBy switch
                {
                    SortBy.Name => filter.SortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Name)
                        : query.OrderByDescending(p => p.Name),
                    SortBy.Category => filter.SortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Category!.Name)
                        : query.OrderByDescending(p => p.Category!.Name),
                    SortBy.CreatedAt => filter.SortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.CreatedAt)
                        : query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };
            }

            var products = await _repo.GetAllAsync(
                filter: filterQuery,
                sort: sortQuery,
                page: filter.Page,
                pageSize: filter.PageSize
                , includeNavigations: true
            );

            var totalCount = await _repo.GetCountAsync(filterQuery);

            return new GenericResponse<PagedResultDto<ProductDto>>
            {
                Result = new PagedResultDto<ProductDto>
                {
                    Items = _mapper.Map<IEnumerable<ProductDto>>(products),
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                },
                Status = HttpStatusCode.OK
            };
        }

        public async Task<GenericResponse<int>> CreateAsync(ProductCreateDto product)
        {

            var existingProduct = await _repo.GetAllAsync(filter: x => x.Where(p => p.Name!.ToString().Trim() == product.Name.Trim()
            && p.CategoryId == product.CategoryId), includeNavigations: true);
            if (existingProduct.Any())
            {
                return new()
                {
                    Result = 0,
                    Message = "Ya existe el producto " + product.Name + " para la categoria " + existingProduct.FirstOrDefault()!.Category!.Name,
                    Status = HttpStatusCode.BadRequest
                };
            }
            var cast = _mapper.Map<Products>(product);
            cast.CreatedAt = DateTime.UtcNow;
            cast.Id = Guid.NewGuid();
            cast.Status = true;
            var result = await _repo.CreateAsync(cast);
            return new()
            {
                Result = result.Id != Guid.Empty ? 1 : 0,
                Message = result.Id != Guid.Empty ? "Se guardó correctamente el producto" : "No se pudo guardar el producto",
                Status = result.Id != Guid.Empty ? HttpStatusCode.OK : HttpStatusCode.BadRequest
            };
        }

        public async Task<GenericResponse<int>> UpdateAsync(Guid id, ProducUpdateDto product)
        {
            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return new GenericResponse<int>
                {
                    Result = 0,
                    Message = $"Producto con ID {id} no encontrado",
                    Status = HttpStatusCode.NotFound
                };
            }

            InjectNonNull<ProducUpdateDto, Products>(product, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _repo.UpdateAsync(existingProduct);
            return new()
            {
                Result = updatedProduct.Id != Guid.Empty ? 1 : 0,
                Message = updatedProduct.Id != Guid.Empty ? "Se actualizó correctamente el producto" : "No se pudo actualizar el producto",
                Status = updatedProduct.Id != Guid.Empty ? HttpStatusCode.OK : HttpStatusCode.BadRequest
            };
        }

        public async Task<GenericResponse<bool>> DeleteAsync(Guid id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists)
            {
                return new GenericResponse<bool>
                {
                    Result = false,
                    Message = $"Producto con ID {id} no encontrado",
                    Status = HttpStatusCode.NotFound
                };
            }

            var result = await _repo.DeleteAsync(id);
            return new()
            {
                Result = result,
                Message = result ? "Se eliminó correctamente el producto" : "No se pudo eliminar producto",
                Status = result ? HttpStatusCode.OK : HttpStatusCode.BadRequest
            };
        }

        private static void InjectNonNull<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProp in sourceProps)
            {
                var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);
                    if (value != null)
                    {
                        targetProp.SetValue(target, value);
                    }
                }
            }
        }
    }

}
