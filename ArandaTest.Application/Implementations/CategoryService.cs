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
    public class CategoryService(IAppRepository<Category> repo, IMapper mapper) : ICategoryService
    {
        private readonly IAppRepository<Category> _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<GenericResponse<CategoryDto>> GetByIdAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id, true);
            if (category == null)
            {

                return new GenericResponse<CategoryDto>
                {
                    Result = null,
                    Message = "Categoria no encontrada.",
                    Status = HttpStatusCode.NotFound
                };
            }

            GenericResponse<CategoryDto> response = new()
            {
                Result = _mapper.Map<CategoryDto>(category),
                Status = HttpStatusCode.OK
            };
            return response;
        }

        public async Task<GenericResponse<PagedResultDto<CategoryDto>>> GetAllAsync(CategoryFilterDto filter)
        {
            IQueryable<Category> filterQuery(IQueryable<Category> query)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(p => p.Name!.Contains(filter.Name));

                return query;
            }

            IOrderedQueryable<Category> sortQuery(IQueryable<Category> query)
            {
                return filter.SortBy switch
                {
                    SortBy.Name => filter.SortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Name)
                        : query.OrderByDescending(p => p.Name),

                    SortBy.CreatedAt => filter.SortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.CreatedAt)
                        : query.OrderByDescending(p => p.CreatedAt),

                    _ => query.OrderBy(p => p.Name)
                };
            }

            var category = await _repo.GetAllAsync(
                filter: filterQuery,
                sort: sortQuery,
                page: filter.Page,
                pageSize: filter.PageSize,
                includeNavigations: true
            );

            var totalCount = await _repo.GetCountAsync(filterQuery);

            return new GenericResponse<PagedResultDto<CategoryDto>>
            {
                Result = new PagedResultDto<CategoryDto>
                {
                    Items = _mapper.Map<IEnumerable<CategoryDto>>(category),
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                },
                Status = HttpStatusCode.OK
            };
        }

        public async Task<GenericResponse<int>> CreateAsync(CategoryDto category)
        {
            var existingCategory = await _repo.GetAllAsync(filter: x => x.Where(p => p.Name.ToString().Trim() == category.Name.Trim()));
            if (existingCategory.Any())
            {
                return new()
                {
                    Result = 0,
                    Message = "Ya existe la categoria " + category.Name,
                    Status = HttpStatusCode.BadRequest
                };
            }

            var cast = _mapper.Map<Category>(category);
            cast.CreatedAt = DateTime.UtcNow;
            cast.Id = Guid.NewGuid();
            cast.IsActive = true;
            var result = await _repo.CreateAsync(cast);
            return new()
            {
                Result = result.Id != Guid.Empty ? 1 : 0,
                Message = result.Id != Guid.Empty ? "Se guardó correctamente la categoria " + category.Name : "No se pudo guardar la categoria " + category.Name,
                Status = result.Id != Guid.Empty ? HttpStatusCode.OK : HttpStatusCode.BadRequest
            };
        }

        public async Task<GenericResponse<int>> UpdateAsync(Guid id, CategoryDto category)
        {
            var existingCategory = await _repo.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return new GenericResponse<int>
                {
                    Result = 0,
                    Message = $"Categoria con ID {id} no encontrado",
                    Status = HttpStatusCode.NotFound
                };
            }

            InjectNonNull<CategoryDto, Category>(category, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;
            existingCategory.Id = id;
            existingCategory.CreatedAt = existingCategory.CreatedAt;


            var updatedCategory = await _repo.UpdateAsync(existingCategory);
            return new()
            {
                Result = updatedCategory.Id != Guid.Empty ? 1 : 0,
                Message = updatedCategory.Id != Guid.Empty ? "Se actualizó correctamente la categoria " + category.Name : "No se pudo actualizar la categoria " + category.Name,
                Status = updatedCategory.Id != Guid.Empty ? HttpStatusCode.OK : HttpStatusCode.BadRequest
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
                        if (value is DateTime dt && dt == DateTime.MinValue)
                            continue;

                        targetProp.SetValue(target, value);
                    }
                }
            }
        }
    }
}
