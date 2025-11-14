using com.InnovaMD.Provider.Models.Log;
using Microsoft.Extensions.Logging;
using System;

namespace com.InnovaMD.Provider.Business
{
    public interface ILogComponent : IDisposable
    {
        void LogAudit(LogAuditEventRequestModel request);
        void LogException(LogExceptionEventRequestModel request, LogLevel level);
    }
}
