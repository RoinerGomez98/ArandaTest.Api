using ArandaTest.Application.DTOs;
using ArandaTest.Domain.Utils;

namespace ArandaTest.Application.Interfaces
{
    public interface IProductService
    {
        Task<GenericResponse<ProductDto>> GetByIdAsync(Guid id);
        Task<GenericResponse<PagedResultDto<ProductDto>>> GetAllAsync(ProductFilterDto filter);
        Task<GenericResponse<int>> CreateAsync(ProductCreateDto product);
        Task<GenericResponse<int>> UpdateAsync(Guid id, ProducUpdateDto product);
        Task<GenericResponse<bool>> DeleteAsync(Guid id);
    }
}
