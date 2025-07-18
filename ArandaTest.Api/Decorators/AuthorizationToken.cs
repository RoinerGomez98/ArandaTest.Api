using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace ArandaTest.Api.Decorators
{
    public class AuthorizationToken(IConfiguration config) : IAuthorizationFilter
    {
        private readonly IConfiguration _config = config;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AllowAnonymousAttribute);

            if (hasAllowAnonymous)
            {
                return;
            }

            var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AuthorizeAttribute);

            if (!hasAuthorizeAttribute)
            {
                return;
            }

            var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var claimsPrincipal = ValidateToken(token, new ApplicationException("Token ha expirado."));
                var userId = claimsPrincipal.FindFirst(ClaimTypes.Authentication)?.Value;

                if (userId != null)
                {
                    context.HttpContext.Session.SetString("UserId", userId);
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }
        }
        private ClaimsPrincipal ValidateToken(string token, Exception applicationException)
        {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"]!,
                    ValidAudience = _config["Jwt:Audience"]!,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);
                return claimsPrincipal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw applicationException;
            }
        }

    }
}
