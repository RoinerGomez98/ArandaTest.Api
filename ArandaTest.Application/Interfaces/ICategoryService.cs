using ArandaTest.Application.DTOs;
using ArandaTest.Domain.Utils;

namespace ArandaTest.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<GenericResponse<CategoryDto>> GetByIdAsync(Guid id);
        Task<GenericResponse<PagedResultDto<CategoryDto>>> GetAllAsync(CategoryFilterDto filter);
        Task<GenericResponse<int>> CreateAsync(CategoryDto product);
        Task<GenericResponse<int>> UpdateAsync(Guid id, CategoryDto product);
    }
}
