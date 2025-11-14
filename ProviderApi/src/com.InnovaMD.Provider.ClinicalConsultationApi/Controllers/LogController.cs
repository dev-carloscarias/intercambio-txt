using com.InnovaMD.Provider.Business;
using com.InnovaMD.Provider.Models.Log;
using com.InnovaMD.Provider.PortalApi.Common;
using com.InnovaMD.Provider.PortalApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace com.InnovaMD.Provider.ClinicalConsultationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : PortalApiControllerBase
    {
        private readonly ILogComponent _logComponent;

        public LogController(ILogComponent logComponent)
        {
            _logComponent = logComponent;
        }

        [Authorize(Policy = nameof(Policies.BearerAuthenticated))]
        [HttpPost("audit")]
        public IActionResult LogAuditEvent([FromBody] LogAuditEventRequestModel request)
        {
            _logComponent.LogAudit(request);
            return new OkResult();
        }

        [Authorize(Policy = nameof(Policies.BearerAuthenticated))]
        [HttpPost("error")]
        public IActionResult LogErrorEvent([FromBody] LogExceptionEventRequestModel request)
        {
            _logComponent.LogException(request, LogLevel.Error);
            return new OkResult();
        }

        [Authorize(Policy = nameof(Policies.BearerAuthenticated))]
        [HttpPost("critical")]
        public IActionResult LogCriticalEvent([FromBody] LogExceptionEventRequestModel request)
        {
            _logComponent.LogException(request, LogLevel.Critical);
            return new OkResult();
        }

    }
}
