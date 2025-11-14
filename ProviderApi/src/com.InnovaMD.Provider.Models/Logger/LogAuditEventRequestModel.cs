using com.InnovaMD.Utilities.Schema.SchemaModels.Definitions;
using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Log
{
    public class LogAuditEventRequestModel
    {
        [Required(ErrorMessage = "AuditEventType is required")]
        public string AuditEventType { get; set; }

        public string AuditEventGroup { get; set; }

        public string Functionality { get; set; }

        public string RequestUrl { get; set; }

        public object EventData { get; set; }

        public User User { get; set; }

        public RequestProperties RequestProperties { get; set; }
    }
}
