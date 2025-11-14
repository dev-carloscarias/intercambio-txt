using com.InnovaMD.Provider.Business.Common;
using com.InnovaMD.Provider.Models.Log;
using com.InnovaMD.Utilities.Logging;
using Microsoft.Extensions.Logging;

namespace com.InnovaMD.Provider.Business
{
    public class LogComponent : BusinessComponentBase, ILogComponent
    {
        private readonly ILogger<LogComponent> _logger;

        public LogComponent(ILogger<LogComponent> logger)
        {
            _logger = logger;
        }

        public void LogAudit(LogAuditEventRequestModel request)
        {
            _logger.LogAuditEvent(request.EventData, request.AuditEventType, request.AuditEventGroup, request.User, request.RequestProperties, request.Functionality);
        }

        public void LogException(LogExceptionEventRequestModel request, LogLevel level)
        {
            _logger.LogWebException(level, request.Message, request.Exception, request.ExceptionType, request.User, request.RequestProperties, request.Functionality);
        }


        #region IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
