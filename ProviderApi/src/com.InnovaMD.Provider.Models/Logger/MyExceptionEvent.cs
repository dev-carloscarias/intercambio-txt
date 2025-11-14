using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Schema.SchemaModels;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Log
{
    public class MyExceptionEvent : ExceptionEvent
    {
        [EnumDataType(typeof(ApplicationDomains))]
        public override string ApplicationDomain { get; set; }
        [EnumDataType(typeof(LogLevel))]
        public override string Level { get; set; }
    }
}
