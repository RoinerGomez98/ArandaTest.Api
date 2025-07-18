using System.Net;
using ArandaTest.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArandaTest.Api.Decorators
{
    public class ErrorsFilterAttribute(ILogger<ErrorsFilterAttribute> logger) : ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorsFilterAttribute> logger = logger;
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            if (context != null)
            {
                Handle(context);
                return Task.CompletedTask;
            }

            throw new ArgumentNullException(nameof(context));
        }
        public override void OnException(ExceptionContext context)
        {
            Handle(context);
        }
        private void Handle(ExceptionContext context)
        {

            logger.LogError(context.Exception, message: context.Exception.Message);

            ObjectResult result = new(new GenericResponse<string>() { Message = "Ha ocurrido un error interno.", Status = HttpStatusCode.InternalServerError })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            context.Result = result;
        }
    }
}
