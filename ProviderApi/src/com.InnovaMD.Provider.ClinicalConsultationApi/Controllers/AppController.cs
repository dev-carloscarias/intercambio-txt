using com.InnovaMD.Provider.Business.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;

namespace com.InnovaMD.Provider.ClinicalConsultationApi.Controllers
{
    public class AppController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public AppController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        [Route("[controller]/index")]
        [Route("index")]
        public IActionResult Index()
        {
            return new OkResult();
        }

        [AllowAnonymous]
        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return new OkResult();
        }


        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is RequestModelValidationException)
            {
                var ex = exceptionHandlerPathFeature?.Error as RequestModelValidationException;
                return new BadRequestObjectResult(ex.Value);
            }
            else if (exceptionHandlerPathFeature?.Error is NotFoundValidationException)
            {
                var ex = exceptionHandlerPathFeature?.Error as NotFoundValidationException;
                return new NotFoundObjectResult(ex.Message);
            }
            else
            {
                if (exceptionHandlerPathFeature?.Error is ParameterTamperingException || exceptionHandlerPathFeature?.Error is CryptographicException)
                {
                    return new NotFoundObjectResult("The action requested cannot be completed.");
                }
                else
                {
                    var result = new ObjectResult(new
                    {
                        IsValid = false,
                        ResponseDescription = "The application has encountered an unknown error.",
                        Exception = _environment.EnvironmentName != Microsoft.Extensions.Hosting.Environments.Production ? exceptionHandlerPathFeature?.Error : null
                    });

                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return result;
                }
            }
        }
    }
}
