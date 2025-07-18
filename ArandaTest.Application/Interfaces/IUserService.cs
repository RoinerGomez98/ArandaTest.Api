using ArandaTest.Application.DTOs;
using ArandaTest.Domain.Utils;

namespace ArandaTest.Application.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponse<IEnumerable<UsersDto>>> GetUsers(UsersFilterDto users);
    }
}
