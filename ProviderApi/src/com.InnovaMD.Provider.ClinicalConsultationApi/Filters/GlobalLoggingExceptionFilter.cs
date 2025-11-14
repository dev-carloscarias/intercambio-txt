using com.InnovaMD.Provider.Business.Exceptions;
using com.InnovaMD.Utilities.Logging.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;

namespace con.InnovaMD.Provider.PortalApi.Services.Filters
{
    public class GlobalLoggingExceptionFilter : ExceptionFilterAttribute
    {
        public GlobalLoggingExceptionFilter()
        {
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is RequestModelValidationException)
            {
                var ex = context.Exception as RequestModelValidationException;
                context.Result = new BadRequestObjectResult(ex.Value);
            }
            else if (context.Exception is NotFoundValidationException)
            {
                var ex = context.Exception as NotFoundValidationException;
                //TODO Evaluate change this line to : context.Result = new HttpNotFoundObjectResult(new ObjectResult( new { IsValid = false, ResponseDescription = ex.Message}));
                context.Result = new NotFoundObjectResult(ex.Message);
            }
            else
            {
                var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<GlobalLoggingExceptionFilter>();
                logger.LogError((int)EventTypes.ExceptionEvent, context.Exception, context.Exception.Message);

                var environment = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();

                if (context.Exception is ParameterTamperingException || context.Exception is CryptographicException)
                {
                    context.Result = new NotFoundObjectResult("The action requested cannot be completed.");
                }
                else
                {
                    var result = new ObjectResult(new
                    {
                        IsValid = false,
                        ResponseDescription = "The application has encountered an unknown error.",
                        Exception = environment.EnvironmentName != Microsoft.Extensions.Hosting.Environments.Production ? context.Exception : null
                    });

                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Result = result;
                }
            }

            base.OnException(context);
        }

    }
}
