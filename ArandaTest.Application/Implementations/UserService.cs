using System.Net;
using ArandaTest.Application.DTOs;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Entities;
using ArandaTest.Domain.Interfaces;
using ArandaTest.Domain.Utils;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace ArandaTest.Application.Implementations
{
    public class UserService(IAppRepository<Users> repo, IMapper mapper, IConfiguration config) : IUserService
    {
        private readonly IAppRepository<Users> _repo = repo;
        private readonly IMapper _mapper = mapper;
        private readonly Security securityC = new(config);

        public async Task<GenericResponse<IEnumerable<UsersDto>>> GetUsers(UsersFilterDto users)
        {
            if (string.IsNullOrEmpty(users.Email))
            {
                return new GenericResponse<IEnumerable<UsersDto>> { Message = "Digite Email.", Status = HttpStatusCode.BadRequest };
            }
            if (string.IsNullOrEmpty(users.Password))
            {
                return new GenericResponse<IEnumerable<UsersDto>> { Message = "Digite Contraseña.", Status = HttpStatusCode.BadRequest };
            }
            var result = await _repo.GetAllAsync(filter: x => x.Where(p => p.Email == users.Email && p.Password == securityC.EncryptP(users.Password) && p.IsActive == true));
            GenericResponse<IEnumerable<UsersDto>> response = new()
            {
                Result = _mapper.Map<IEnumerable<UsersDto>>(result),
                Status = HttpStatusCode.OK
            };
            return response;

        }
    }
}
