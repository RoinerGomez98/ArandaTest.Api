using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ArandaTest.Application.DTOs;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ArandaTest.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService, IConfiguration config) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IConfiguration _config = config;

        [HttpPost, AllowAnonymous]
        [Route("Authentication")]
        public async Task<GenericResponse<UsersDto>> Authentication(UsersFilterDto genericRequest)
        {

            var res = await _userService.GetUsers(genericRequest);
            if (res.Status == HttpStatusCode.BadRequest)
            {
                return new GenericResponse<UsersDto> { Message = res.Message, Status = res.Status };
            }
            UsersDto response = res.Result!.FirstOrDefault()!;
            GenericResponse<UsersDto> tk;
            if (response != null)
            {
                tk = GenerateToken(new UserClaims { Email = response.Email, Document = response.Document, Name = response!.Name, Id = response.Id.ToString() }, response);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new GenericResponse<UsersDto> { Message = "No existe usuario con el email o contraseña digitada", Status = HttpStatusCode.BadRequest };
            }

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return new GenericResponse<UsersDto> { Result = tk.Result, Status = HttpStatusCode.OK };
        }

        private GenericResponse<UsersDto> GenerateToken(UserClaims userClaims, UsersDto userResponse)
        {
            if (userClaims == null)
                return new GenericResponse<UsersDto>();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Authentication, userClaims.Id!),
                new Claim(ClaimTypes.Actor, userClaims.Document!),
                new Claim(ClaimTypes.NameIdentifier, userClaims.Name!),
                new Claim(ClaimTypes.Email, userClaims.Email!),
            };

            DateTime expira = DateTime.Now.AddHours(1);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: expira,
              signingCredentials: credentials);

            userResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
            userResponse.Expires = expira;

            return new GenericResponse<UsersDto> { Result = userResponse };
        }

    }
}
